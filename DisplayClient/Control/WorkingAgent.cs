using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace DisplayClient
{
    public class WorkingAgent
    {
        private SocketHandler socketHandler;

        private bool alive;

        public WorkingAgent(RenderConfiguration configuration)
        {
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

        public async Task<RenderMessage> Connect(Agent agent)
        {
            Client c = new Client();

            StreamSocket socket = await c.Connect(agent.IP, NetworkConfiguration.Port);

            SocketHandler handler = new SocketHandler(socket);

            // send render job request
            RCS_Render_Job jobRequest = new RCS_Render_Job(this.Configuration);

            byte[] sendData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(jobRequest);

            handler.SendMessage(MessageCode.MC_Render_Job, sendData);

            // receive render job response
            SocketHandler.SocketMessage socketMsg = await handler.WaitForMessage();
            
            if (!socketMsg.Empty)
            {
                RCS_Render_Job_Message jobResponse = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Message>(socketMsg.Content);

                if (jobResponse.Message == RenderMessage.Supported)
                {
                    this.Agent = agent;
                    this.socketHandler = new SocketHandler(socket);

                    this.socketHandler.OnMessageBytesReceived += SocketHandler_OnMessageBytesReceived;
                    this.socketHandler.OnConnectionLost += SocketHandler_OnConnectionLost;

                    this.alive = true;
                }
                else
                {
                    this.socketHandler.Close();
                }

                return jobResponse.Message;
            }

            throw new AgentNotReachableException("The agent could not be found!");
        }

        private void SocketHandler_OnConnectionLost()
        {
            if (this.OnAgentGotUnreachable != null)
            {
                this.OnAgentGotUnreachable(this);
            }
        }

        public void CancelRenderJob()
        {
            RCS_Render_Job_Cancel cancelRequest = new RCS_Render_Job_Cancel(CancelRenderJobReason.Manually, Configuration.RenderJobID);

            this.socketHandler.SendMessage(MessageCode.MC_Render_Job_Cancel, Remote_Content_Show_MessageGenerator.GetMessageAsByte(cancelRequest));
            this.socketHandler.Close();

            this.socketHandler.OnMessageBytesReceived -= this.SocketHandler_OnMessageBytesReceived;
        }
        
        private void KeepAlive()
        {
            Task.Factory.StartNew(() =>
            {
                this.alive = false;

                Task.Delay(1000 * 60);

                if (this.alive)
                {
                    RCS_Alive alive = new RCS_Alive();

                    this.socketHandler.SendMessage(MessageCode.MC_Alive, Remote_Content_Show_MessageGenerator.GetMessageAsByte(alive));
                }
                else
                {
                    this.socketHandler.Close();

                    if (this.OnAgentGotUnreachable != null)
                    {
                        this.OnAgentGotUnreachable(this);
                    }
                }
            });
        }

        private void SocketHandler_OnMessageBytesReceived(MessageCode code, byte[] bytes)
        {
            if (code == MessageCode.MC_Alive)
            {
                RCS_Alive result = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Alive>(bytes);

                this.alive = true;
            }
            else if (code == MessageCode.MC_Render_Job_Result)
            {
                RCS_Render_Job_Result result = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Result>(bytes);

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
