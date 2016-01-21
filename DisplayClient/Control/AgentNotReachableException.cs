using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayClient
{
    public class AgentNotReachableException : Exception
    {
        public AgentNotReachableException(string text) : base(text)
        {
        }
    }
}
