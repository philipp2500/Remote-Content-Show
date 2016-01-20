using Remote_Content_Show_Protocol;
using System;
using System.Net.Sockets;
using System.Threading;


// ***************************************************************
// TODO: ERROR HANDLING
// ***************************************************************


namespace Agent.Network
{
    public class ClientHandler
    {
        private TcpClient client = null;
        private NetworkStream stream = null;
        private ListenerThreadArgs args = null;

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            this.stream = this.client.GetStream();
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
            this.stream.Close();
            this.client.Close();
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
                    case Remote_Content_Show_Container.MessageCode.MC_Process_List_Request:
                        RCS_Process_List_Request request = (RCS_Process_List_Request)
                            Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Process_List_Request>(contentBuffer, header);

                        //TODO

                        break;
                    case Remote_Content_Show_Container.MessageCode.MC_Render_Job:
                        RCS_Render_Job jobRequest = (RCS_Render_Job)
                            Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job>(contentBuffer, header);
                        
                        //TODO

                        break;
                    case Remote_Content_Show_Container.MessageCode.MC_Render_Job_Cancel:
                        RCS_Render_Job_Cancel cancelRequest = (RCS_Render_Job_Cancel)
                            Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Render_Job_Cancel>(contentBuffer, header);

                        //TODO

                        break;
                    case Remote_Content_Show_Container.MessageCode.MC_Alive:
                        RCS_Alive aliveMsg = (RCS_Alive)
                            Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Alive>(contentBuffer, header);

                        //TODO

                        break;
                }
            }
        }
    }
}
