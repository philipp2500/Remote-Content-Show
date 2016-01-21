using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace DisplayClient.DisplayManager
{
    public class ExternalAdmin
    {
        private SocketHandler socketHandler;

        public ExternalAdmin(StreamSocket socket)
        {
            this.socketHandler = new SocketHandler(socket);

            this.socketHandler.OnMessageBytesReceived += SocketHandler_OnMessageBytesReceived;
        }

        public delegate void JobConfigurationReceived(Job_Configuration configuration);

        public delegate void CancelRequestReceived(Guid jobID, CancelJobReason reason);

        public delegate void EventRequestReceived();

        public event JobConfigurationReceived OnJobConfigurationReceived;

        public event CancelRequestReceived OnCancelRequestReceived;

        public event EventRequestReceived OnEventRequestReceived;

        private void SocketHandler_OnMessageBytesReceived(MessageCode code, byte[] bytes)
        {
            if (code == MessageCode.MC_Job)
            {
                RCS_Job job = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Job>(bytes);

                if (this.OnJobConfigurationReceived != null)
                {
                    this.OnJobConfigurationReceived(job.Configuration);
                }
            }
            else if (code == MessageCode.MC_Job_Cancel)
            {
                RCS_Job_Cancel cancel = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Job_Cancel>(bytes);

                if (this.OnCancelRequestReceived != null)
                {
                    this.OnCancelRequestReceived(cancel.JobID, cancel.Reason);
                }
            }
            else if (code == MessageCode.MC_Event_List_Request)
            {
                RCS_Event_List_Request events = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Event_List_Request>(bytes);

                if (this.OnEventRequestReceived != null)
                {
                    this.OnEventRequestReceived();
                }
            }
            else if (code == MessageCode.MC_Configuration_Image)
            {

            }
        }
    }
}
