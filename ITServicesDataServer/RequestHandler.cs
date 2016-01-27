using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ITServicesDataServer
{
    public class RequestHandler
    {
        public RequestHandler(string request, IPAddress ip)
        {
            this.Request = request;
            this.IP = ip;
        }

        public IPAddress IP
        {
            get;
            private set;
        }

        public string Request
        {
            get;
            private set;
        }
    }
}
