using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NearDubDetect
{
    class NearDubDetector
    {

        public NearDubDetector()
        {
            randomList = GenerateRandomIntegers();
        }
        List<Website> knownwebsites = new List<Website>();
        List<Int32> randomList = new List<int>();

        public List<Shingle> FindShingles(string textinput)
        {
            string text = HtmlRemoval.StripTagsRegex(textinput);
            string fixedText = Regex.Replace(text, "[^a-zA-Z0-9% -]", string.Empty);
            string[] textsplit = fixedText.Split(' ');

            List<Shingle> returnlist = new List<Shingle>();

            for (int i = 3; i < textsplit.Length; i++)
            {
                Shingle temp = new Shingle();
                temp.words.Add(textsplit[i - 3]);
                temp.words.Add(textsplit[i - 2]);
                temp.words.Add(textsplit[i - 1]);
                temp.words.Add(textsplit[i]);
                returnlist.Add(temp);
            }

            return returnlist;
        }

        public List<int> HashShingles(string input)
        {
            double h = 0;
            double t;
            string temp;
            List<Shingle> shingles = FindShingles(input);
            List<Int32> returnlist = new List<int>();
            foreach (Shingle shingle in shingles)
            {
                foreach (string word in shingle.words)
                {
                    t = 0;
                    MD5 md5 = System.Security.Cryptography.MD5.Create();

                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(word);

                    byte[] hash = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString());
                    }

                    temp = sb.ToString();

                    foreach (char ch in temp)
                    {
                        t += Convert.ToInt32(ch);
                    }

                    h += t;
                }
                returnlist.Add(Convert.ToInt32(h));
                h = 0;
            }

            return returnlist;
        }

        public List<int> BigShiftHash(List<int> input, int randomint)
        {
            List<int> returnlist = new List<int>();
            foreach (int item in input)
            {
                returnlist.Add(item ^ randomint);
            }
            return returnlist;
        }

        public List<Int32> GenerateRandomIntegers()
        {
            List<Int32> liste = new List<Int32>();
            Random RNG = new Random();
            for (int i = 0; i < 84; i++)
            {
                liste.Add(RNG.Next(0, 1000000));
            }
            return liste;
        }

        public double Jaccard(Website input1, Website input2)
        {
            string input1Content = input1.HTMLContent;
            string input2Content = input2.HTMLContent;
            List<int> input1HashedShingles = new List<int>();
            List<int> input2HashedShingles = new List<int>();
            List<int> Sketch1 = new List<int>(84);
            List<int> Sketch2 = new List<int>(84);

            if (input1.HTMLContent == "" || input1.HTMLContent == null)
            {
                if (input2.HTMLContent == "" || input2.HTMLContent == null)
                {
                    return 100;
                }
                else
                {
                    return 0;
                }
            }
            if (input2.HTMLContent == "" || input2.HTMLContent == null || input1.HTMLContent.Split(' ').Length < 4 || input2.HTMLContent.Split(' ').Length < 6)
            {
                return 0;
            }

            if (knownwebsites.Contains(input1))
            {
                Sketch1 = input1.Sketch;
            }
            else
            {
                input1HashedShingles = HashShingles(input1Content);
                for (int i = 0; i < randomList.Count; i++)
                {
                    Sketch1.Add(BigShiftHash(input1HashedShingles, randomList[i]).Min());
                }
                input1.Sketch = Sketch1;
                knownwebsites.Add(input1);
            }

            if (knownwebsites.Contains(input2) && input2.Sketch != null)
            {
                Sketch2 = input2.Sketch;
            }
            else
            {
                input2HashedShingles = HashShingles(input2Content);
                for (int i = 0; i < randomList.Count; i++)
                {
                    Sketch2.Add(BigShiftHash(input2HashedShingles, randomList[i]).Min());
                }
                input2.Sketch = Sketch2;
                knownwebsites.Add(input2);
            }

            double identicalcounter = 0;

            for (int i = 0; i < Sketch1.Count - 1; i++)
            {
                if (Sketch1[i] == Sketch2[i])
                {
                    identicalcounter++;
                }
            }

            IEnumerable<int> union = Sketch1.Union(Sketch2);            

            return (identicalcounter / union.Count()) * 100;

        }
    }
}