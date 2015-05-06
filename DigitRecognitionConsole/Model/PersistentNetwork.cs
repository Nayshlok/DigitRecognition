using DigitRecognitionConsole.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    [Serializable]
    public class PersistentNetwork
    {
        public NeuralNet Network { get; set; }
        public int Index { get; set; }
    }
}
