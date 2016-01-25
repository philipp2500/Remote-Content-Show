using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class FileResource : IResource
    {
        public bool Loacal
        {
            get;
            set;
        } 

        
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }
    }
}
