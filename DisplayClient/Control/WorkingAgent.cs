using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace DisplayClient
{
    public class WorkingAgent
    {
        public static readonly int CONNECT_TIMEOUT = 4000;

        public static readonly int KEEP_ALIVE_SEND_INTERVAL = 5000;

        public static readonly int KEEP_ALIVE_WAIT_INTERVAL = 10000;

        private SocketHandler socketHandler;

        private long lastKeepAlive;

        public WorkingAgent(RenderConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public delegate void MessageReceived(WorkingAgent agent, RCS_Render_Job_Message message);

        public delegate void ResultReceived(WorkingAgent agent, RCS_Render_Job_Result result);

        public delegate void AgentGotUnreachable(WorkingAgent agent);

        public event MessageReceived OnMessageReceived;

        public event ResultReceived OnResultReceived;

        public event AgentGotUnreachable OnAgentGotUnreachable;

        public RenderConfiguration Configuration
        {
            get;
            private set;
        }

        public Agent Agent
        {
            get;
            private set;
        }

        public bool Working
        {
            get;
            private set;
        }

        public async Task<RenderMessage> Connect(Agent agent)
        {
            Client c = new Client();
            StreamSocket socket;

            try
            {
                socket = await c.Connect(agent.IP, NetworkConfiguration.PortAgent, CONNECT_TIMEOUT);
            }
            catch (Exception)
            {
                throw new AgentNotReachableException("The agent could not be found!");
            }

            // send render job request
            RCS_Render_Job jobRequest = new RCS_Render_Job(this.Configuration, RemoteType.Client);

            byte[] sendData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(jobRequest);

            this.socketHandler = new SocketHandler(socket);

            this.socketHandler.SendMessage(MessageCode.MC_Render_Job, sendData);

            // receive render job response
            SocketHandler.SocketMessage socketMsg;
            socketMsg.Code = MessageCode.MC_Alive;
            socketMsg.Content = new byte[] { };
            socketMsg.Empty = true;

            socketMsg = await this.socketHandler.WaitForMessage();

            // if he sends some invalid data, give him another chance.
            if (!socketMsg.Empty && socketMsg.Code != MessageCode.MC_Render_Job_Message)
            {
                socketMsg = await this.socketHandler.WaitForMessage();
            }

            if (!socketMsg.Empty)
            {
                RCS_Render_Job_Message jobResponse = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Message>(socketMsg.Content);

                if (jobResponse.Message == RenderMessage.Supported)
                {
                    this.Agent = agent;
                    //this.socketHandler = new SocketHandler(socket);

                    this.socketHandler.OnMessageBytesReceived += SocketHandler_OnMessageBytesReceived;
                    this.socketHandler.OnConnectionLost += SocketHandler_OnConnectionLost;
                    this.socketHandler.Start();

                    this.Working = true;

                    this.lastKeepAlive = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    this.KeepAlive();
                }

                return jobResponse.Message;
            }

            // okay, he is not able to communicate.
            this.socketHandler.Close();

            return RenderMessage.NotSupported;
        }

        private void SocketHandler_OnConnectionLost()
        {
            this.Working = false;

            if (this.OnAgentGotUnreachable != null)
            {
                this.OnAgentGotUnreachable(this);
            }
        }

        public void CancelRenderJob()
        {
            this.Working = false;

            RCS_Render_Job_Cancel cancelRequest = new RCS_Render_Job_Cancel(CancelRenderJobReason.Manually, Configuration.RenderJobID, RemoteType.Client);

            this.socketHandler.SendMessage(MessageCode.MC_Render_Job_Cancel, Remote_Content_Show_MessageGenerator.GetMessageAsByte(cancelRequest));
            this.socketHandler.Close();

            this.socketHandler.OnMessageBytesReceived -= this.SocketHandler_OnMessageBytesReceived;
            this.socketHandler.OnConnectionLost -= this.SocketHandler_OnConnectionLost;
        }
        
        private async void KeepAlive()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (this.Working)
                {
                    RCS_Alive alive = new RCS_Alive(RemoteType.Client);

                    this.socketHandler.SendMessage(MessageCode.MC_Alive, Remote_Content_Show_MessageGenerator.GetMessageAsByte(alive));

                    await Task.Delay(KEEP_ALIVE_SEND_INTERVAL);
                }
            });

            await Task.Factory.StartNew(async () =>
            {
                while (this.Working)
                {
                    await Task.Delay(KEEP_ALIVE_WAIT_INTERVAL);

                    if ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - this.lastKeepAlive > 10000)
                    {
                        this.Working = false;
                        this.socketHandler.Close();

                        if (this.OnAgentGotUnreachable != null)
                        {
                            this.OnAgentGotUnreachable(this);
                        }
                    }
                }
            });
        }

        private void SocketHandler_OnMessageBytesReceived(MessageCode code, byte[] bytes)
        {
            if (code == MessageCode.MC_Alive)
            {
                RCS_Alive result = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Alive>(bytes);

                this.lastKeepAlive = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;                
            }
            else if (code == MessageCode.MC_Render_Job_Result)
            {
                RCS_Render_Job_Result result = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Result>(bytes);

                //Debug.Write("result size: " + bytes.Length);

                if (this.OnResultReceived != null)
                {
                    this.OnResultReceived(this, result);
                }
            }
            else if (code == MessageCode.MC_Render_Job_Message)
            {
                RCS_Render_Job_Message message = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Message>(bytes);

                if (this.OnMessageReceived != null)
                {
                    this.OnMessageReceived(this, message);
                }
            }
        }
    }
}
