using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DisplayClient.Log
{
    [DataContract]
    public class LoggedEvent
    {
        [DataMember]
        public Job_EventType Type
        {
            get;
            set;
        }

        [DataMember]
        public string Description
        {
            get;
            set;
        }

        [DataMember]
        public DateTime Time
        {
            get;
            set;
        }

        [DataMember]
        public LoggedJob ConcernedJob
        {
            get;
            set;
        }
    }
}
