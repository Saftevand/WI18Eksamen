using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearDubDetect
{
    public class Domain
    {
        public string URL;
        public Restriction restriction;
        public DateTime LastVisited;

        public Domain(string url, Restriction restrictioninput)
        {
            URL = url;
            restriction = restrictioninput;
        }

        
    }
}
