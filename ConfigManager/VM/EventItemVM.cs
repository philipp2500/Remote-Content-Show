using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace ConfigManager
{
    public class EventItemVM
    {
        private Event eventEntry;

        public EventItemVM(Event eventEntry)
        {
            this.eventEntry = eventEntry;
        }

        public Job_EventType Type
        {
            get
            {
                return this.eventEntry.Type;
            }
        }

        public string Description
        {
            get
            {
                return this.eventEntry.Description;
            }
        }

        public DateTime Time
        {
            get
            {
                return this.eventEntry.Time;
            }
        }

        public string NameOfConcernedJob
        {
            get
            {
                return this.eventEntry.NameOfConcernedJob;
            }
        }
    }
}
