using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITServicesDataServer
{
    public class ReslutItem
    {
        public DateTime Datum
        {
            get;
            set;
        }

        public TimeSpan Von
        {
            get;
            set;
        }

        public TimeSpan Bis
        {
            get;
            set;
        }

        public string Saal
        {
            get;
            set;
        }

        public string Bezeichnung
        {
            get;
            set;
        }

        public string LVArt
        {
            get;
            set;
        }

        public string LVBezeichnung
        {
            get;
            set;
        }
    }
}
