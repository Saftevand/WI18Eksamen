using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NearDubDetect
{
    class QueryTool
    {
        private Crawler _crawler;

        public QueryTool()
        {

        }
        public QueryTool(Crawler crawler)
        {
            _crawler = crawler;
        }

        private List<string> stopWords = new List<string>(new string[] { "-","?",":",";","a","about","above","after","again","against","all","am","an","and","any","are","aren't","as","at","be","because","been","before","being","below","between","both","but"
        ,"by"
        ,"can't"
        ,"cannot"
        ,"could"
        ,"couldn't"
        ,"did"
        ,"didn't"
        ,"do"
        ,"does"
        ,"doesn't"
        ,"doing"
        ,"don't"
        ,"down"
        ,"during"
        ,"each"
        ,"few"
        ,"for"
        ,"from"
        ,"further"
        ,"had"
        ,"hadn't"
        ,"has"
        ,"hasn't"
        ,"have"
        ,"haven't"
        ,"having"
        ,"he"
        ,"he'd"
        ,"he'll"
        ,"he's"
        ,"her"
        ,"here"
        ,"here's"
        ,"hers"
        ,"herself"
        ,"him"
        ,"himself"
        ,"his"
        ,"how"
        ,"how's"
        ,"i"
        ,"i'd"
        ,"i'll"
        ,"i'm"
        ,"i've"
        ,"if"
        ,"in"
        ,"into"
        ,"is"
        ,"isn't"
        ,"it"
        ,"it's"
        ,"its"
        ,"itself"
        ,"let's"
        ,"me"
        ,"more"
        ,"most"
        ,"mustn't"
        ,"my"
        ,"myself"
        ,"no"
        ,"nor"
        ,"not"
        ,"of"
        ,"off"
        ,"on"
        ,"once"
        ,"only"
        ,"or"
        ,"other"
        ,"ought"
        ,"our"
        ,"ours ourselves"
        ,"out"
        ,"over"
        ,"own"
        ,"same"
        ,"shan't"
        ,"she"
        ,"she'd"
        ,"she'll"
        ,"she's"
        ,"should"
        ,"shouldn't"
        ,"so"
        ,"some"
        ,"such"
        ,"than"
        ,"that"
        ,"that's"
        ,"the"
        ,"their"
        ,"theirs"
        ,"them"
        ,"themselves"
        ,"then"
        ,"there"
        ,"there's"
        ,"these"
        ,"they"
        ,"they'd"
        ,"they'll"
        ,"they're"
        ,"they've"
        ,"this"
        ,"those"
        ,"through"
        ,"to"
        ,"too"
        ,"under"
        ,"until"
        ,"up"
        ,"very"
        ,"was"
        ,"wasn't"
        ,"we"
        ,"we'd"
        ,"we'll"
        ,"we're"
        ,"we've"
        ,"were"
        ,"weren't"
        ,"what"
        ,"what's"
        ,"when"
        ,"when's"
        ,"where"
        ,"where's"
        ,"which"
        ,"while"
        ,"who"
        ,"who's"
        ,"whom"
        ,"why"
        ,"why's"
        ,"with"
        ,"won't"
        ,"would"
        ,"wouldn't"
        ,"you"
        ,"you'd"
        ,"you'll"
        ,"you're"
        ,"you've"
        ,"your"
        ,"yours"
        ,"yourself"
        ,"yourselves"});
        private Dictionary<string, DfWrapper> incidenceVector = new Dictionary<string, DfWrapper>();

        // MP 2
        public List<string> GenerateTokensAndInvertedIndex(string input, int idOfDocument)
        {
            string lowerCaseInput = input.ToLower();
            List<string> words = new List<string>(lowerCaseInput.Split(' '));
            words.RemoveAll(e => stopWords.Exists(sw => sw.Equals(e)));
            List<string> tokens = new List<string>();
            foreach (string word in words)
            {
                tokens.Add(PorterStemming(word));
            }

            foreach (string token in tokens)
            {
                if (incidenceVector.ContainsKey(token))
                {
                    Document tempDoc = incidenceVector[token].PostingList.Find(x => x.Id == idOfDocument);

                    if (tempDoc == null)
                    {
                        Document temp = new Document(idOfDocument);
                        incidenceVector[token].DocFreq++;
                        incidenceVector[token].PostingList.Add(temp);
                        temp.TermFrequency++;
                    }
                    else
                    {
                        incidenceVector[token].DocFreq++;
                        tempDoc.TermFrequency++;
                    }
                }
                else
                {
                    incidenceVector.Add(token, new DfWrapper(1, new Document(idOfDocument)));
                    incidenceVector[token].PostingList.First().TermFrequency++;
                }
            }

            return tokens;
        }

        // MP 2
        public List<Document> PassQueryBoolean(string inputQuery)
        {
            List<string> disectedQuery = new List<string>(inputQuery.ToLower().Split(' '));
            disectedQuery.RemoveAll(e => stopWords.Exists(sw => sw.Equals(e)));

            List<Document> foundPages = new List<Document>();

            for (int i = 0; i < disectedQuery.Count; i++)
            {
                if (disectedQuery[i] == "*and*")
                {
                    if (disectedQuery[i+1] == "*not*")
                    {
                        foreach (var item in incidenceVector[disectedQuery[i+2]].PostingList)
                        {
                            try
                            {
                                foundPages.Remove(item);
                            }
                            catch (Exception)
                            {}
                        }
                    }
                    else
                    {
                        foundPages = foundPages.Union(incidenceVector[disectedQuery[i+1]].PostingList).ToList();
                    }
                }
                else if (disectedQuery[i] == "*or*")
                {
                    if (disectedQuery[i + 1] == "*not*")
                    {
                        foreach (var item in incidenceVector)
                        {
                            if (!(item.Key == disectedQuery[i+2]))
                            {
                                foundPages.AddRange(item.Value.PostingList);
                            }
                        }
                    }
                    else
                    {
                        foundPages.AddRange(incidenceVector[disectedQuery[i + 1]].PostingList);
                    }
                }
                else if (disectedQuery[i] == disectedQuery[0])
                {
                    foundPages.AddRange(incidenceVector[disectedQuery[i]].PostingList);
                }
                else if (disectedQuery[0] == "*not*")
                {
                    foreach (var item in incidenceVector)
                    {
                        if (!(item.Key == disectedQuery[1]))
                        {
                            foundPages.AddRange(item.Value.PostingList);
                        }
                    }
                }
            }
            

            /*
            List<string> foundTokens = new List<string>();
            List<Document> blacklistedDocuments = new List<Document>();
            int queryTokenCounter = 0;
            foreach (string queryWord in disectedQuery)
            {
                if (queryWord == "*not*")
                {
                    if (queryTokenCounter != disectedQuery.Count)
                    {
                        blacklistedDocuments.AddRange(incidenceVector[disectedQuery[queryTokenCounter + 1]].PostingList);
                    }
                }
                else if (incidenceVector.ContainsKey(queryWord) && !foundTokens.Contains(queryWord))
                {
                    foundTokens.Add(queryWord);
                }
                queryTokenCounter++;
            }

            List<Document> foundPages = new List<Document>();
            //foundPages.DocumentList.AddRange(ANDpageFinder(foundTokens));
            foundPages.AddRange(ORpageFinder(foundTokens));
            foundPages = foundPages.Distinct().ToList();
            foundPages.RemoveAll(e => blacklistedDocuments.Contains(e));
            */
            List<Document> returlist = new List<Document>();
            
            foreach (Document doc in foundPages)
            {
                if (!returlist.Exists(x => x.Id == doc.Id))
                {
                    returlist.Add(doc);
                }
            }


            

            return returlist;

        }

        // MP 3
        public List<KeyValuePair<int, double>> passQueryContent(string inputQuery, int numberOfDocuments)
        {
            List<string> query = new List<string>(inputQuery.ToLower().Split(' '));
            query.RemoveAll(e => stopWords.Exists(sw => sw.Equals(e)));

            CalculateTf_Idf(numberOfDocuments);

            Dictionary<int, List<double>> DocVectors = new Dictionary<int, List<double>>();
            int queryTermCounter = 0;

            foreach (string term in query)
            {
                if (incidenceVector.ContainsKey(term))
                {
                    foreach (Document item in incidenceVector[term].PostingList)
                    {
                        if (DocVectors.ContainsKey(item.Id))
                        {
                            DocVectors[item.Id].Add(item.TfIdf);
                        }
                        else
                        {
                            DocVectors.Add(item.Id, new List<double>());
                            DocVectors[item.Id].Add(item.TfIdf);
                        }
                    }
                    queryTermCounter++;

                    foreach (var item in DocVectors)
                    {
                        if (item.Value.Count < queryTermCounter)
                        {
                            item.Value.Add(0);
                        }
                    }
                }
            }

            if (DocVectors.Count == 0)
            {
                return new List<KeyValuePair<int, double>>();
            }

            Dictionary<string, double> termfreq = new Dictionary<string, double>();

            foreach (var term in query)
            {
                termfreq.Add(term, 0);
                foreach (var term2 in query)
                {
                    if (term == term2)
                    {
                        termfreq[term]++;
                    }
                }
            }
            List<double> tf_query = new List<double>();
            foreach (var term in termfreq)
            {
                tf_query.Add(1 + Math.Log10(term.Value));
            }

            List<double> idf_query = new List<double>();

            foreach (var term in query)
            {
                idf_query.Add(Math.Log10(numberOfDocuments / incidenceVector[term].PostingList.Count));
            }

            List<double> TfIdf_query = new List<double>();

            for (int i = 0; i < tf_query.Count; i++)
            {
                TfIdf_query.Add(tf_query[i] * idf_query[i]);
            }


            //Cosine similarity score

            List<KeyValuePair<int, double>> scores = new List<KeyValuePair<int, double>>();

            foreach (var item in DocVectors)
            {
                scores.Add(new KeyValuePair<int, double>(item.Key, item.Value.Zip(TfIdf_query, (x, y) => x * y).Sum()));
            }

            var score = from entry in scores orderby entry.Value descending select entry;
            List<KeyValuePair<int, double>> result = new List<KeyValuePair<int, double>>();

            foreach (var item in score)
            {
                    result.Add(item);
            }

            return result;
        }

        // MP 4
        public List<KeyValuePair<int, double>> passQueryPageRank()
        {  
            double[] pageranks = _crawler.PageRanker(0.10, 200);
            List<KeyValuePair<int, double>> docsWithPageRank = new List<KeyValuePair<int, double>>();

            for (int i = 0; i < pageranks.Length; i++)
            {
                docsWithPageRank.Add(new KeyValuePair<int, double>(i, pageranks[i]));
            }

            return docsWithPageRank;
            /*
            List<KeyValuePair<Document, double>> final = new List<KeyValuePair<Document, double>>();

            for (int i = 0; i < pageranks.Length; i++)
            {
                KeyValuePair<Document, double> temp = cosscore.Find(x => x.Key.Id == i);
                if (temp.Key != null)
                {
                    final.Add(new KeyValuePair<Document, double>(temp.Key, pageranks[i] * temp.Value));
                }
            }
            
            final.Sort((x,y) => x.Value.CompareTo(y.Value));
            final.Reverse();
            try
            {
                final = final.GetRange(0, 10);
            }
            catch (Exception)
            {
            }            

            return final;*/
        }

        //MP 4
        public List<KeyValuePair<int, double>> passQueryContentAndPageRank(List<KeyValuePair<int, double>> pagerank, List<KeyValuePair<int, double>> Content)
        {
            double weightPagerank = 0.5;
            double weightContent = 0.5;

            List<KeyValuePair<int, double>> scoredDocs = new List<KeyValuePair<int, double>>();

            var TempContentSortedOnID = from entry in Content orderby entry.Key ascending select entry;
            List<KeyValuePair<int, double>> ContentSortedOnID = new List<KeyValuePair<int, double>>();

            foreach (var item in TempContentSortedOnID)
            {
                ContentSortedOnID.Add(item);
            }

            foreach (var pr in pagerank)
            {
                foreach (var cont in Content)
                {
                    if (pr.Key ==  cont.Key)
                    {
                        scoredDocs.Add(new KeyValuePair<int, double>(pr.Key, (pagerank[pr.Key].Value * weightPagerank + ContentSortedOnID.Find(x => x.Key == pr.Key).Value * weightContent)));
                    }
                }
            }

            var TempResult = from entry in scoredDocs orderby entry.Value descending select entry;
            List<KeyValuePair<int, double>> Result = new List<KeyValuePair<int, double>>();

            foreach (var item in TempResult)
            {
                Result.Add(item);
            }

            return Result;

        }
             
        // MP 2
        public List<Document> ANDpageFinder(List<string> queryTokens)
        {
            List<Document> commonIndexes = new List<Document>();
            List<Document> firstDocIndexes = new List<Document>(incidenceVector[queryTokens[0]].PostingList);
            if (queryTokens.Count > 1)
            {
                commonIndexes = incidenceVector[queryTokens[1]].PostingList.Intersect(firstDocIndexes).ToList();
                for (int i = 2; i < queryTokens.Count; i++)
                {
                    commonIndexes = incidenceVector[queryTokens[i]].PostingList.Intersect(commonIndexes).ToList();
                }
                return commonIndexes;
            }
            else return firstDocIndexes;
        }

        // MP 2
        public List<Document> ORpageFinder(List<string> queryTokens)
        {
            List<Document> pageIndexes = new List<Document>();
            foreach (string queryToken in queryTokens)
            {
                pageIndexes.AddRange(incidenceVector[queryToken].PostingList);
            }
            pageIndexes.Sort((x, y) =>
            {
                if (pageIndexes.Count(e => e == x) < pageIndexes.Count(e => e == y)) return 1;
                else if (pageIndexes.Count(e => e == x) > pageIndexes.Count(e => e == y)) return -1;
                else return 0;
            });

            pageIndexes = pageIndexes.Distinct().ToList();

            return pageIndexes;
        }

        // MP 2
        public string PorterStemming(string inputWord)
        {
            Regex ssPattern = new Regex("sses$");
            Regex iesPattern = new Regex("ies");
            Regex ationalPattern = new Regex("ational");
            Regex tionalPattern = new Regex("tional");
            ssPattern.Replace(inputWord, "ss");
            iesPattern.Replace(inputWord, "i");
            ationalPattern.Replace(inputWord, "ate");
            tionalPattern.Replace(inputWord, "tion");

            return inputWord;

        }

        // MP 3
        public void CalculateTf_Idf(int totalDocs)
        {
            foreach (KeyValuePair<string, DfWrapper> pair in incidenceVector)
            {
                foreach (Document doc in pair.Value.PostingList)
                {
                    doc.TfIdf = (1 + Math.Log10(doc.TermFrequency)) * Math.Log10(Convert.ToDouble( totalDocs) /Convert.ToDouble( pair.Value.DocFreq));
                }
            }
        }
    }
}



