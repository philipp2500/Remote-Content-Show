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
    public class ExternalAdmin
    {
        private SocketHandler socketHandler;

        public ExternalAdmin(StreamSocket socket)
        {
            this.socketHandler = new SocketHandler(socket);

            this.socketHandler.OnMessageBytesReceived += SocketHandler_OnMessageBytesReceived;
            this.socketHandler.Start();
        }

        public delegate void JobConfigurationReceived(Job_Configuration configuration);

        public delegate void CancelRequestReceived(Guid jobID, CancelJobReason reason);

        public delegate void ConfigurationImageReceived(byte[] image);

        // Need to know the sender to send a response back.
        public delegate void EventRequestReceived(ExternalAdmin sender);

        public event JobConfigurationReceived OnJobConfigurationReceived;

        public event CancelRequestReceived OnCancelRequestReceived;

        public event ConfigurationImageReceived OnConfigurationImageReceived;

        public event EventRequestReceived OnEventRequestReceived;

        // file events

        public delegate void LocalFileListRequestReceived(ExternalAdmin sender);

        public delegate void LocalFileAddRequestReceived(byte[] content, string filename);

        public delegate void LocalFileRemoveRequestReceived(string filename);

        public event LocalFileListRequestReceived OnLocalFileListRequestReceived;

        public event LocalFileAddRequestReceived OnLocalFileAddRequestReceived;

        public event LocalFileRemoveRequestReceived OnLocalFileRemoveRequestReceived;

        public void SendEventsList(Event_List events)
        {
            RCS_Event_List_Response response = new RCS_Event_List_Response(events, RemoteType.Client);

            this.socketHandler.SendMessage(MessageCode.MC_Event_List_Response, Remote_Content_Show_MessageGenerator.GetMessageAsByte(response));

            byte[] deppen = Remote_Content_Show_MessageGenerator.GetMessageAsByte(response);

            RCS_Event_List_Response deppen2 = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Event_List_Response>(deppen);

            this.socketHandler.Close();
        }

        public void SendLocalFilesList(RCS_FileList msg)
        {
            this.socketHandler.SendMessage(MessageCode.MC_FileList, Remote_Content_Show_MessageGenerator.GetMessageAsByte(msg));

            this.socketHandler.Close();
        }

        private void SocketHandler_OnMessageBytesReceived(MessageCode code, byte[] bytes)
        {
            if (code == MessageCode.MC_Job)
            {
                RCS_Job job = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Job>(bytes);

                this.socketHandler.Close();

                if (this.OnJobConfigurationReceived != null)
                {
                    this.OnJobConfigurationReceived(job.Configuration);
                }
            }
            else if (code == MessageCode.MC_Job_Cancel)
            {
                RCS_Job_Cancel cancel = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Job_Cancel>(bytes);

                this.socketHandler.Close();

                if (this.OnCancelRequestReceived != null)
                {
                    this.OnCancelRequestReceived(cancel.JobID, cancel.Reason);
                }
            }
            else if (code == MessageCode.MC_Configuration_Image)
            {
                RCS_Configuration_Image image = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Configuration_Image>(bytes);

                this.socketHandler.Close();

                if (this.OnConfigurationImageReceived != null)
                {
                    this.OnConfigurationImageReceived(image.Picture);
                }
            }
            else if (code == MessageCode.MC_Event_List_Request)
            {
                RCS_Event_List_Request events = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Event_List_Request>(bytes);

                if (this.OnEventRequestReceived != null)
                {
                    this.OnEventRequestReceived(this);
                }
            }
            else if (code == MessageCode.MC_FileAdd)
            {
                RCS_FileAdd newFile = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_FileAdd>(bytes);

                this.socketHandler.Close();

                if (this.OnLocalFileAddRequestReceived != null)
                {
                    this.OnLocalFileAddRequestReceived(newFile.Data, newFile.Name);
                }

            }
            else if (code == MessageCode.MC_FileDelete)
            {
                RCS_FileDelete deleteFile = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_FileDelete>(bytes);

                this.socketHandler.Close();

                if (this.OnLocalFileRemoveRequestReceived != null)
                {
                    this.OnLocalFileRemoveRequestReceived(deleteFile.Name);
                }
            }
            else if (code == MessageCode.MC_GetFiles)
            {
                RCS_GetFiles listRequest = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_GetFiles>(bytes);

                if (this.OnLocalFileListRequestReceived != null)
                {
                    this.OnLocalFileListRequestReceived(this);
                }
            }
        }
    }
}
