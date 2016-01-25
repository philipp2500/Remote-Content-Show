using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public static class Remote_Content_Show_MessageGenerator
    {

        public static byte[] GetMessageAsByte(object message)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, Formatting.Indented, settings));
        }

        public static T GetMessageFromByte<T>(byte[] data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data), settings);
        }

        //switch (header.Code)
        //    {
        //        case MessageCode.MC_Alive:
        //            return JsonConvert.DeserializeObject<>(Encoding.UTF8.GetString(data));
        //            break;
        //        case MessageCode.MC_Configuration_Image:
        //            break;
        //        case MessageCode.MC_Event_List_Request:
        //            break;
        //        case MessageCode.MC_Event_List_Response:
        //            break;
        //        case MessageCode.MC_Job:
        //            break;
        //        case MessageCode.MC_Job_Cancel:
        //            break;
        //        case MessageCode.MC_Process_List_Request:
        //            break;
        //        case MessageCode.MC_Process_List_Response:
        //            break;
        //        case MessageCode.MC_Render_Job:
        //            break;
        //        case MessageCode.MC_Render_Job_Cancel:
        //            break;
        //        case MessageCode.MC_Render_Job_Error:
        //            break;
        //        case MessageCode.MC_Render_Job_Result:
        //            break;
        //    }
    }
}
