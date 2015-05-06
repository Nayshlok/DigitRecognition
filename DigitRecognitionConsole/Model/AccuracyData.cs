using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class AccuracyData
    {
        public int correct;
        public int total;

        public override string ToString()
        {
            return ((double)(correct) / (double)(total)) * 100 + "%, Correct: " + correct + ", Given: " + total;
        }
    }
}
