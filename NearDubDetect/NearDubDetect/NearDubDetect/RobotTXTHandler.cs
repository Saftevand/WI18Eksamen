using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace NearDubDetect
{
    class RobotTXTHandler
    {       

        public Restriction FindRestrictions(string website)
        {

            Restriction restrictions = new Restriction();
            string webData = "";
            bool scope = false;
            WebClient wc = new WebClient();
            try
            {
                byte[] raw = wc.DownloadData(website + "/robots.txt");
                webData = Encoding.UTF8.GetString(raw).ToLower();
            }
            catch (Exception)
            {

            }
            string[] temp;
            List<string> result = new List<string>();

            string[] lines = webData.Split('\n');
            foreach (string item in lines)
            {
                if (item.Contains("user-agent: *"))
                {
                    scope = true;
                }
                else if (item.Contains("user-agent: "))
                {
                    scope = false;
                }

                if (scope == true)
                {
                    if (item.Contains("allow"))
                    {
                        temp = item.Split(' ');
                        restrictions.allow.Add(temp[1]);
                    }
                    else if (item.Contains("disallow"))
                    {
                        temp = item.Split(' ');
                        restrictions.disallow.Add(temp[1]);
                    }
                    else if(item.Contains("crawl-delay"))
                    {
                        temp = item.Split(' ');
                        restrictions.crawldelay = Convert.ToInt32(temp[1]);
                    }
                }
            }

            return restrictions;
        }
    }
}
