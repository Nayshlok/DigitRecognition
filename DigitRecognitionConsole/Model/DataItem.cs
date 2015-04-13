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
    }
}
