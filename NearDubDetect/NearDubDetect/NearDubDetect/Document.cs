using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearDubDetect
{
    class Document
    {
        private int _id;
        private int _termFrequency;
        private double _tfIdf;

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int TermFrequency
        {
            get
            {
                return _termFrequency;
            }

            set
            {
                _termFrequency = value;
            }
        }


        public double TfIdf
        {
            get
            {
                return _tfIdf;
            }

            set
            {
                _tfIdf = value;
            }
        }

        public Document(int id)
        {
            this.Id = id;
        }

        public override bool Equals(object obj)
        {
            if (this._id == (obj as Document).Id)
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
