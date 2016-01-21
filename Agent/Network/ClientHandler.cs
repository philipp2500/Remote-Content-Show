using Remote_Content_Show_Protocol;
using System;
using System.Net.Sockets;
using System.Threading;
using Remote_Content_Show_Container;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;


// ***************************************************************
// TODO: ERROR HANDLING
// ***************************************************************


namespace Agent.Network
{
    public class ClientHandler
    {
        /// <summary>
        /// The size of the preview image contained in the <see cref="Remote_Content_Show_Protocol.RCS_Process_List_Response"/>.
        /// </summary>
        private Size previewImageSize = new Size(100, 100);
        private TcpClient client = null;
        private NetworkStream stream = null;
        private ListenerThreadArgs args = null;
        private Dictionary<Guid, ScreenCapture> runningRenderJobs = null;

        /// <summary>
        /// The event fired when the connected client disconnects.
        /// </summary>
        public event EventHandler OnClientDisconnected;

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            this.stream = this.client.GetStream();
            this.runningRenderJobs = new Dictionary<Guid, ScreenCapture>();
        }

        public void Start()
        {
            this.args = new ListenerThreadArgs();
            Thread listenerThread = new Thread(new ParameterizedThreadStart(this.HandleMessages));
            listenerThread.Start(this.args);
        }

        public void Stop()
        {
            this.args.Exit = true;

            try { this.stream.Close(); }
            catch { }

            try { this.client.Close(); }
            catch { }
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
                    this.stream.Read(headerBuffer, 0, Remote_Content_Show_Header.HeaderLength);

                    if (!Remote_Content_Show_Header.IsValidHeader(headerBuffer))
                    {
                        continue;
                    }

                    header = Remote_Content_Show_Header.FromByte(headerBuffer);
                    contentBuffer = new byte[header.Length];
                    this.stream.Read(contentBuffer, 0, (int)header.Length);

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

                            this.HandleJobRequest(jobRequest);
                            //TODO

                            break;
                        case MessageCode.MC_Render_Job_Cancel:
                            RCS_Render_Job_Cancel cancelRequest =
                                Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Cancel>(contentBuffer);

                            //TODO

                            break;
                        case MessageCode.MC_Alive:
                            RCS_Alive aliveMsg =
                                Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Alive>(contentBuffer);
                            
                            //TODO

                            break;
                    }
                }
            }
            catch (IOException)
            {
                this.Stop();

                if (this.OnClientDisconnected != null)
                {
                    this.OnClientDisconnected(this, EventArgs.Empty);
                }
            }
        }
        
        /// <summary>
        /// Responds with a list of the running processes that have a window.
        /// </summary>
        private void HandleProcessListRequest()
        {
            RCS_Process_List_Response response =
                new RCS_Process_List_Response(this.GetProcessList(this.previewImageSize), Environment.MachineName);

            byte[] responseMsg = Remote_Content_Show_MessageGenerator.GetMessageAsByte(response);
            byte[] responseHeader =
                new Remote_Content_Show_Header(MessageCode.MC_Process_List_Response, responseMsg.Length).ToByte;

            this.stream.Write(responseHeader, 0, responseHeader.Length);
            this.stream.Write(responseMsg, 0, responseMsg.Length);
            this.stream.Flush();
        }

        private void HandleJobRequest(RCS_Render_Job jobRequest)
        {
            ScreenCapture capturer = new ScreenCapture();
            capturer.OnImageCaptured += Capturer_OnImageCaptured;
            capturer.OnCaptureFinished += Capturer_OnCaptureFinished;
            capturer.OnProcessExited += Capturer_OnProcessExited;

            this.runningRenderJobs.Add(jobRequest.Configuration.RenderJobID, capturer);

            capturer.StartCapture(
                Process.GetProcessesByName("devenv")[0], 
                (int)jobRequest.Configuration.UpdateInterval,
                jobRequest.Configuration.JobToDo.Duration, 
                jobRequest.Configuration.RenderJobID);
            //TODO not devenv.exe
        }

        private void Capturer_OnProcessExited(object sender, EventArgs e)
        {
            // TODO
            // TODO send back when captured process exits
        }

        private void Capturer_OnCaptureFinished(object sender, CaptureFinishEventArgs e)
        {
            this.runningRenderJobs.Remove(e.RenderJobId);
        }

        private void Capturer_OnImageCaptured(object sender, ImageEventArgs e)
        {
            byte[] img = ImageHandler.ImageHandler.ImageToBytes(e.Image);
            byte[] msg = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_Render_Job_Result(e.ConcernedRenderJobID, img));
            byte[] header = new Remote_Content_Show_Header(MessageCode.MC_Render_Job_Result, msg.Length).ToByte;

            this.stream.Write(header, 0, header.Length);
            this.stream.Write(msg, 0, msg.Length);
            this.stream.Flush();
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

                Bitmap bmp = capturer.CaptureWindow(proc.MainWindowHandle);
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
    }
}
