using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class DataItem
    {
        public double[] data { get; set; }
        public int expectedResult { get; set; }

        public override string ToString()
        {
            StringBuilder dataString = new StringBuilder();
            bool isFirst = true;
            foreach (byte b in data)
            {
                if (!isFirst)
                {
                    dataString.Append(", ");
                }
                dataString.Append(b);
                isFirst = false;
            }
            return ", Expect: " + expectedResult;
        }

        public override bool Equals(object obj)
        {
            bool areEqual = false;

            if (obj is DataItem)
            {
                DataItem other = obj as DataItem;
                areEqual = Enumerable.SequenceEqual(this.data, other.data) && this.expectedResult == other.expectedResult;
            }

            return areEqual;
        }

        //public override int GetHashCode()
        //{
        //    return 1001;
        //}
    }
}
