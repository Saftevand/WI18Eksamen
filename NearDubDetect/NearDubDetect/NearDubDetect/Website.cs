using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearDubDetect
{
    public class Website
    {
        public Website(Domain domain, string url)
        {
            DomainURL = domain;
            currentPath = url;
        }
        public Domain DomainURL;
        public string currentPath;
        public string HTMLContent;
        public List<int> Sketch;
        public List<Website> LinkedFrom = new List<Website>();

        public override bool Equals(Object obj)
        {
            if ((obj as Website).currentPath == this.currentPath)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}