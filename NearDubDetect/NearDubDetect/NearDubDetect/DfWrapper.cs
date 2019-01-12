using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearDubDetect
{
    class DfWrapper
    {
        private int _df;
        private List<Document> _documentList = new List<Document>();

        public int DocFreq
        {
            get
            {
                return _df;
            }

            set
            {
                _df = value;
            }
        }

        public List<Document> PostingList
        {
            get
            {
                return _documentList;
            }

            set
            {
                _documentList = value;
            }
        }

        public DfWrapper()
        { }

        public DfWrapper(int DF, Document doc)
        {
            this.DocFreq = DF;
            PostingList.Add(doc);
        }

    }
}
