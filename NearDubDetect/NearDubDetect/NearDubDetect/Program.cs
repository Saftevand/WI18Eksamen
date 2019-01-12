using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearDubDetect
{
    class Program
    {
        static void Main(string[] args)
        {
            testCrawler();
        }

        static void testCrawler()
        {
            Crawler crawler = new Crawler("https://heste-nettet.dk/");
            crawler.Crawl(100);
            QueryTool queryMaster = new QueryTool(crawler);
            int websiteCounter = 0;
            foreach (var item in crawler.websites)
            {
                try
                {
                    string noHtmlString = HtmlRemoval.MasterStripper(item.HTMLContent);
                    queryMaster.GenerateTokensAndInvertedIndex(noHtmlString, websiteCounter);

                    websiteCounter++;
                }
                catch(Exception e)
                {
                    //Console.WriteLine(e.Message);
                }
            }

            List<KeyValuePair<int, double>> pagerank = queryMaster.passQueryPageRank();

            Console.WriteLine("\n Boolean: \n");
            List<Document> pageResultsBoolean = queryMaster.PassQueryBoolean("dansk *and* ride *not* forbund");
            foreach (Document doc in pageResultsBoolean.Take(10))
            {
                Console.WriteLine(crawler.websites[doc.Id].currentPath);
            }

            Console.WriteLine("\n Content: \n");
            List<KeyValuePair<int, double>> pageResultsContent = queryMaster.passQueryContent("dansk ride forbund", websiteCounter);
            foreach (KeyValuePair<int, double> doc in pageResultsContent.Take(10))
            {
                Console.WriteLine(crawler.websites[doc.Key].currentPath);
            }

            Console.WriteLine("\n Content with pagerank: \n");
            List<KeyValuePair<int, double>> pageResultsContentAndPageRank = queryMaster.passQueryContentAndPageRank(pagerank, pageResultsContent);
            foreach (KeyValuePair<int, double> doc in pageResultsContentAndPageRank.Take(10))
            {
                Console.WriteLine(crawler.websites[doc.Key].currentPath);
            }


            foreach (Website page in crawler.websites)
            {
                Console.WriteLine(page.currentPath);
            }

            Console.WriteLine(crawler.websites.Count);
            Console.ReadLine();
        }


        static void testQueryTool()
        {
            string test1 = @"League of Legends (abbreviated LoL) is a multiplayer online battle arena video game developed and published by Riot Games for Microsoft Windows and macOS. The game follows a freemium model and is supported by microtransactions, and was inspired by the Warcraft III: The Frozen Throne mod, Defense of the Ancients.[1]
In League of Legends, players assume the role of an unseen summoner that controls a champion with unique abilities and battle against a team of other players or computer - controlled champions.The goal is usually to destroy the opposing team's nexus, a structure which lies at the heart of a base protected by defensive structures, although other distinct game modes exist as well. Each League of Legends match is discrete, with all champions starting off fairly weak but increasing in strength by accumulating items and experience over the course of the game.[2] The champions and setting blend a variety of elements, including high fantasy, steampunk, and Lovecraftian horror.
League of Legends was generally well received upon its release in 2009, and has since grown in popularity, with an active and expansive fanbase. By July 2012, League of Legends was the most played PC game in North America and Europe in terms of the number of hours played.[3] As of January 2014, over 67 million people played League of Legends per month, 27 million per day, and over 7.5 million concurrently during peak hours.[4] League has among the largest footprints of any game in streaming media communities on platforms such as YouTube and Twitch.tv; it routinely ranks first in the most-watched hours.[5][6] In September 2016 the company estimated that there are over 100 million active players each month.[7][8] The game's popularity has led it to expand into merchandise, with toys, accessories, apparel, as well as tie-ins to other media through music videos, web series, documentaries, and books.
League of Legends has an active and widespread competitive scene. In North America and Europe, Riot Games organizes the League Championship Series(LCS), located in Los Angeles and Berlin respectively, which consists of 10 professional teams in each continent.[9] Similar regional competitions exist in China(LPL), South Korea(LCK), Taiwan/Hong Kong/Macau(LMS) and various other regions.These regional competitions culminate with the annual World Championship. The 2017 World Championship had 60 million unique viewers and a total prize pool of over 4 million USD.[10][11] The 2018 Mid-Season Invitational had an overall peak concurrent viewership of 19.8 million, while the finals had an average concurrent viewership of 11 million";
            string test2 = "In North America and Europe, Riot Games organizes the League Championship Series(LCS), located in Los Angeles and Berlin respectively, which consists of 10 professional teams in each continent.[9] Similar regional competitions exist in China(LPL), South Korea(LCK), Taiwan/Hong Kong/Macau(LMS) and various other regions.These regional competitions culminate with the annual World Championship. The 2017 World Championship had 60 million unique viewers and a total prize pool of over 4 million USD.[10][11] The 2018 Mid-Season Invitational had an overall peak concurrent viewership of 19.8 million, while the finals had an average concurrent viewership of 11 million";
            QueryTool queryMaster = new QueryTool();
            queryMaster.GenerateTokensAndInvertedIndex(test1, 0);
            queryMaster.GenerateTokensAndInvertedIndex(test2, 1);
            queryMaster.PassQueryBoolean("League *NOT* Legends");

            Console.ReadKey();
        }
    }
}
