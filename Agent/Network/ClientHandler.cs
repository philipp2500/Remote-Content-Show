using Remote_Content_Show_Protocol;
using Remote_Content_Show_Container;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace Agent.Network
{
    public class ClientHandler
    {
        /// <summary>
        /// The time in milliseconds between sending <see cref="RCS_Alive"/> messages.
        /// </summary>
		private const int KEEP_ALIVE_INTERVAL = 5000;
        /// <summary>
        /// The number of <see cref="ClientHandler.KEEP_ALIVE_INTERVAL"/>s before the connection is aborted.
        /// </summary>
        private const int ALLOWED_KEEP_ALIVE_MISSES = 5;

        /// <summary>
        /// The size of the preview image contained in the <see cref="Remote_Content_Show_Protocol.RCS_Process_List_Response"/>.
        /// </summary>
        private Size previewImageSize = new Size(100, 100);
        private TcpClient client = null;
        private NetworkStream stream = null;
        private ListenerThreadArgs args = null;
        private Dictionary<Guid, ScreenCapture> runningRenderJobs = null;
        private Timer keepAliveSendTimer = null;
        private Timer keepAliveCheckTimer = null;
        private DateTime lastKeepAliveTime;

        /// <summary>
        /// The event fired when the connected client disconnects or this <see cref="ClientHandler"/> is stopped.
        /// </summary>
        public event EventHandler OnClientDisconnected;

        /// <summary>
        /// The event fired when no <see cref="RCS_Alive"/> messages were received and the connection was cut.
        /// <seealso cref="ClientHandler.ALLOWED_KEEP_ALIVE_MISSES"/>.
        /// </summary>
        public event EventHandler OnKeepAliveOmitted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientHandler"/> class.
        /// </summary>
        /// <param name="client">The client whose messages to handle.</param>
        /// <exception cref="InvalidOperationException">The System.Net.Sockets.TcpClient is not connected to a remote host.</exception>
        /// <exception cref="ObjectDisposedException">The System.Net.Sockets.TcpClient has been closed.</exception>
        public ClientHandler(TcpClient client)
        {
            this.client = client;
            this.stream = this.client.GetStream();
            this.runningRenderJobs = new Dictionary<Guid, ScreenCapture>();
        }

        /// <summary>
        /// Starts listening for messages.
        /// </summary>
        public void Start()
        {
            this.args = new ListenerThreadArgs();
            Thread listenerThread = new Thread(new ParameterizedThreadStart(this.HandleMessages));
            listenerThread.Start(this.args);
        }

        /// <summary>
        /// Stops listening for messages, stops all jobs, stops sending <see cref="RCS_Alive"/> messages
        /// and disconnects from the remote client.
        /// </summary>
        public void Stop()
        {
            this.args.Exit = true;

            foreach (var job in this.runningRenderJobs.Values)
            {
                job.StopCapture();
            }

            if (this.keepAliveSendTimer != null)
            {
                this.keepAliveSendTimer.Dispose();
                this.keepAliveSendTimer = null;
            }

            if (this.keepAliveCheckTimer != null)
            {
                this.keepAliveCheckTimer.Dispose();
                this.keepAliveCheckTimer = null;
            }
            
            try { this.stream.Close(); }
            catch { }

            try { this.client.Close(); }
            catch { }

            if (this.OnClientDisconnected != null)
            {
                this.OnClientDisconnected(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles following messages received from a given client:
        /// <see cref="Remote_Content_Show_Container.MessageCode.MC_Process_List_Request"/>,
        /// <see cref="Remote_Content_Show_Container.MessageCode.MC_Render_Job"/>,
        /// <see cref="Remote_Content_Show_Container.MessageCode.MC_Render_Job_Cancel"/>,
        /// <see cref="Remote_Content_Show_Container.MessageCode.MC_Alive"/>.
        /// </summary>
        /// <param name="data">The arguments of type <see cref="ListenerThreadArgs"/> used to be able to stop the thread.</param>
        private void HandleMessages(object data)
        {
            ListenerThreadArgs args = data as ListenerThreadArgs;

            if (args == null)
            {
                throw new ArgumentException("Parameter must be of type ListenerThreadArgs and must not be null.");
            }

            Remote_Content_Show_Header header = null;
            byte[] headerBuffer = new byte[Remote_Content_Show_Header.HeaderLength];
            byte[] contentBuffer = null;

            try
            {
                while (!args.Exit)
                {
                    if (!this.stream.DataAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    this.stream.Read(headerBuffer, 0, Remote_Content_Show_Header.HeaderLength);

                    if (!Remote_Content_Show_Header.IsValidHeader(headerBuffer))
                    {
                        continue;
                    }

                    header = Remote_Content_Show_Header.FromByte(headerBuffer);
                    contentBuffer = new byte[header.Length];
                    this.stream.Read(contentBuffer, 0, (int)header.Length);

                    // only send Keep Alive messages to client
                    if (this.keepAliveSendTimer == null && header.Remote == RemoteType.Client)
                    {
                        this.keepAliveSendTimer = new Timer(new TimerCallback(this.SendKeepAlive), null, 0, KEEP_ALIVE_INTERVAL);
                        this.keepAliveCheckTimer = new Timer(new TimerCallback(this.CheckKeepAlive), null, 0, KEEP_ALIVE_INTERVAL);
                        this.lastKeepAliveTime = DateTime.Now;
                    }

                    switch (header.Code)
                    {
                        case MessageCode.MC_Process_List_Request:
                            RCS_Process_List_Request request =
                                Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Process_List_Request>(contentBuffer);

                            this.HandleProcessListRequest();

                            // cut connection after handling of the process list request
                            this.Stop();
                            break;
                        case MessageCode.MC_Render_Job:
                            RCS_Render_Job jobRequest =
                                Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job>(contentBuffer);

                            if (jobRequest == null)
                            {
                                //TODO when other Agent is not able to render, we receive a correct job request and
                                // another job request header which is not followed by a correct job.
                                break;
                            }
                            
                            this.HandleJobRequest(jobRequest);

                            break;
                        case MessageCode.MC_Render_Job_Cancel:
                            RCS_Render_Job_Cancel cancelRequest =
                                Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Cancel>(contentBuffer);

                            this.HandleCancelRequest(cancelRequest);

                            break;
                        case MessageCode.MC_Alive:
							// no further handling required
                            break;
                    }
                }
            }
            catch (IOException)
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Stops capturing images for the given render job.
        /// </summary>
        /// <param name="cancelRequest">The message containing the canceled render job's GUID.</param>
        private void HandleCancelRequest(RCS_Render_Job_Cancel cancelRequest)
        {
            if (!this.runningRenderJobs.ContainsKey(cancelRequest.ConcernedRenderJobID))
            {
                return;
            }

            ScreenCapture capturer = this.runningRenderJobs[cancelRequest.ConcernedRenderJobID];

            capturer.StopCapture();
            this.runningRenderJobs.Remove(cancelRequest.ConcernedRenderJobID);
        }

        /// <summary>
        /// Responds with a list of the running processes that have a window.
        /// </summary>
        private void HandleProcessListRequest()
        {
            RCS_Process_List_Response response =
                new RCS_Process_List_Response(this.GetProcessList(this.previewImageSize), Environment.MachineName, RemoteType.Agent);

            byte[] responseMsg = Remote_Content_Show_MessageGenerator.GetMessageAsByte(response);
            byte[] responseHeader =
                new Remote_Content_Show_Header(MessageCode.MC_Process_List_Response, responseMsg.Length, RemoteType.Agent).ToByte;

            try
            {
                this.stream.Write(responseHeader, 0, responseHeader.Length);
                this.stream.Write(responseMsg, 0, responseMsg.Length);
                this.stream.Flush();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends a message indicating whether this agent is able to execute the job.
        /// If this agent is able, starts capturing images as configured in the given render job.
        /// </summary>
        /// <param name="jobRequest">The render job to execute.</param>
        private void HandleJobRequest(RCS_Render_Job jobRequest)
        {
            // check if this agent is able to execute the render job
            if (!this.CheckExecutionAbility(jobRequest))
            {
                return;
            }

            ScreenCapture capturer = new ScreenCapture();
            capturer.OnImageCaptured += Capturer_OnImageCaptured;
            capturer.OnCaptureFinished += Capturer_OnCaptureFinished;
            capturer.OnProcessExited += Capturer_OnProcessExited;

            this.runningRenderJobs.Add(jobRequest.Configuration.RenderJobID, capturer);

            try
            {
                capturer.StartCapture(jobRequest.Configuration);
            }
            catch (Exception ex)
            when (ex is InvalidOperationException ||
                  ex is ArgumentException ||
                  ex is ProcessStartupException)
            {
            }
        }

        /// <summary>
        /// Checks if this agent is able to execute the given render job and sends an according message to the client.
        /// </summary>
        /// <param name="renderJob">The render job to execute</param>
        /// <returns>True if this agent is able to execute the job, false otherwise.</returns>
        private bool CheckExecutionAbility(RCS_Render_Job renderJob)
        {
            bool capable = false;
            RCS_Render_Job_Message msg = null;
            IResource resource = renderJob.Configuration.JobToDo.Resource;

            if (renderJob == null ||
                renderJob.Configuration == null ||
                renderJob.Configuration.JobToDo == null)
            {
                return false;
            }

            if (resource is FileResource)
            {
                // check if a program exists which can open the file
                string filepath = ((FileResource)resource).Path;
                capable = ProgramFinder.FindExecutable(filepath) != string.Empty;
            }
            else if (resource is ProcessResource)
            {
                // check if process with given ID is running
                int pid = ((ProcessResource)resource).ProcessID;

                try
                {
                    Process.GetProcessById(pid);
                    capable = true;
                }
                catch
                {
                }
            }
            else
            {
                throw new NotSupportedException("Only FileResource and ProcessResource are supported.");
            }

            if (capable)
            {
                msg = new RCS_Render_Job_Message(RenderMessage.Supported, renderJob.Id, RemoteType.Agent);
            }
            else
            {
                msg = new RCS_Render_Job_Message(RenderMessage.NotSupported, renderJob.Id, RemoteType.Agent);
            }
            
            try
            {
                this.SendMessage(MessageCode.MC_Render_Job_Message, msg);
            }
            catch
            {
            }

            return capable;
        }

        /// <summary>
        /// Sends a message to the client indicating that the captured process has exited.
        /// </summary>
        private void Capturer_OnProcessExited(object sender, CaptureFinishEventArgs e)
        {
            this.runningRenderJobs.Remove(e.RenderJobId);

            var msg = new RCS_Render_Job_Message(RenderMessage.ProcessExited, e.RenderJobId, RemoteType.Agent);

            try
            {
                this.SendMessage(MessageCode.MC_Render_Job_Message, msg);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Removes the finished render job from the list of running jobs.
        /// </summary>
        private void Capturer_OnCaptureFinished(object sender, CaptureFinishEventArgs e)
        {
            this.runningRenderJobs.Remove(e.RenderJobId);
        }

        /// <summary>
        /// Sends the captured image to the client.
        /// </summary>
        private void Capturer_OnImageCaptured(object sender, ImageEventArgs e)
        {
            byte[] img = ImageHandler.ImageHandler.ImageToBytes(e.Image);
            var msg = new RCS_Render_Job_Result(e.ConcernedRenderJobID, img, RemoteType.Agent);

            try
            {
                this.SendMessage(MessageCode.MC_Render_Job_Result, msg);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Returns a list of the running processes that have a window (except this app's process and explorer.exe).
        /// </summary>
        /// <param name="previewImageSize">The size of the preview images for the processes.</param>
        private Process_List GetProcessList(Size previewImageSize)
        {
            ScreenCapture capturer = new ScreenCapture();
            Process_List processes = new Process_List();
            processes.Processes = new List<ProcessDescription>();

            foreach (var proc in Process.GetProcesses())
            {
                if (proc.MainWindowHandle == IntPtr.Zero || 
                    proc.Id == Process.GetCurrentProcess().Id ||
                    proc.ProcessName == "explorer")
                {
                    continue;
                }

                Bitmap bmp = capturer.SnapshotWindow(proc.MainWindowHandle);
                if (bmp == null)
                {
                    // only give access to processes which provide a window
                    continue;
                }

                // convert the resized picture to bytes
                byte[] picture = ImageHandler.ImageHandler.ImageToBytes(ImageHandler.ImageHandler.Resize(bmp, previewImageSize));

                ProcessDescription desc = new ProcessDescription(picture, proc.Id, proc.ProcessName, proc.MainWindowTitle);
                processes.Processes.Add(desc);
            }

            return processes;
        }

        private void SendKeepAlive(object state)
        {
            try
            {
                this.SendMessage(MessageCode.MC_Alive, new RCS_Alive(RemoteType.Agent));
            }
            catch (Exception ex)
            when (ex is IOException ||
                  ex is ObjectDisposedException)
            {
                this.Stop();
            }
        }

        private void CheckKeepAlive(object state)
        {
            if (this.lastKeepAliveTime.AddMilliseconds(KEEP_ALIVE_INTERVAL * ALLOWED_KEEP_ALIVE_MISSES) < DateTime.Now)
            {
                if (this.OnKeepAliveOmitted != null)
                {
                    this.OnKeepAliveOmitted(this, EventArgs.Empty);
                }

                this.Stop();
            }
        }

        /// <summary>
        /// Sends a header and the given message to the remote client.
        /// </summary>
        /// <param name="msgCode">The type of message to send.</param>
        /// <param name="msg">The message to send.</param>
        /// <exception cref="IOException">
        /// There was a failure while writing to the network. -or-
        /// An error occurred when accessing the socket.</exception>
        /// <exception cref="ObjectDisposedException">
        /// The System.Net.Sockets.NetworkStream is closed. -or-
        /// There was a failure reading from the network.</exception>
        private void SendMessage(MessageCode msgCode, Remote_Content_Show_Message msg)
        {
            byte[] byteMsg = Remote_Content_Show_MessageGenerator.GetMessageAsByte(msg);
            byte[] header = new Remote_Content_Show_Header(msgCode, byteMsg.Length, RemoteType.Agent).ToByte;

            lock (this.stream)
            {
                this.stream.Write(header, 0, header.Length);
                this.stream.Write(byteMsg, 0, byteMsg.Length);
                this.stream.Flush();
            }
        }
    }
}
