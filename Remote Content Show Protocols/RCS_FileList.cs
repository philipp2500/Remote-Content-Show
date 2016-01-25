using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_FileList : Remote_Content_Show_Message
    {
        public RCS_FileList(List<FileItem> items, RemoteType remote) : base(remote)
        {
            this.Items = items;
        }

        public List<FileItem> Items
        {
            get;
            private set;
        }
    }
}