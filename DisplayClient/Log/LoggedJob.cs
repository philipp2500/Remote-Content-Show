using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DisplayClient.Log
{
    [DataContract]
    public class LoggedJob
    {
        [DataMember]
        public Guid JobID
        {
            get;
            set;
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }
    }
}
