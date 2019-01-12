using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearDubDetect
{
    public class Restriction
    {
        public List<string> allow = new List<string>();
        public List<string> disallow = new List<string>();
        public int crawldelay = 0;
    }
}
