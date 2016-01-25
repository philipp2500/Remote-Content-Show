using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public enum MessageCode : int
    {
        MC_Alive = 10,

        MC_Job = 20,

        MC_Job_Cancel = 21,

        MC_Event_List_Request = 30,

        MC_Event_List_Response = 31,

        MC_Process_List_Request = 40,

        MC_Process_List_Response = 41,

        MC_Render_Job = 50,

        MC_Render_Job_Cancel = 51,

        MC_Render_Job_Message = 52,

        MC_Render_Job_Result = 53,

        MC_Configuration_Image = 60
    }
}
