using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class DataItem
    {
        public byte[] data { get; set; }
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
            return dataString.ToString() + ", Expect: " + expectedResult;
        }
    }
}
