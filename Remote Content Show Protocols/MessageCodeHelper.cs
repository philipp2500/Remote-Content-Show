using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Remote_Content_Show_Container.Enums;

namespace Remote_Content_Show_Protocol
{
    public static class MessageCodeHelper
    {
        public static Type GetType(MessageCode code)
        {
            return GetMap()[code];
        }

        public static MessageCode GetMessageCode(Type type)
        {
            return GetMap().First(x => x.Value.Equals(type)).Key;
        }

        private static Dictionary<MessageCode, Type> GetMap()
        {
            Dictionary<MessageCode, Type> dict = new Dictionary<MessageCode, Type>();

            dict.Add(MessageCode.MC_Alive, typeof(RCS_Alive));
            dict.Add(MessageCode.MC_Job, typeof(RCS_Job));
            dict.Add(MessageCode.MC_Job_Cancel, typeof(RCS_Job_Cancel));
            dict.Add(MessageCode.MC_Event_List_Request, typeof(RCS_Event_List_Request));
            dict.Add(MessageCode.MC_Event_List_Response, typeof(RCS_Event_List_Response));
            dict.Add(MessageCode.MC_Process_List_Request, typeof(RCS_Process_List_Request));
            dict.Add(MessageCode.MC_Process_List_Response, typeof(RCS_Process_List_Response));
            dict.Add(MessageCode.MC_Render_Job, typeof(RCS_Render_Job));
            dict.Add(MessageCode.MC_Render_Job_Cancel, typeof(RCS_Render_Job_Cancel));
            dict.Add(MessageCode.MC_Render_Job_Error, typeof(RCS_Render_Job_Error));
            dict.Add(MessageCode.MC_Render_Job_Result, typeof(RCS_Render_Job_Result));
            dict.Add(MessageCode.MC_Configuration_Image, typeof(RSC_Configuration_Image));

            return dict;
        }
    }
}
