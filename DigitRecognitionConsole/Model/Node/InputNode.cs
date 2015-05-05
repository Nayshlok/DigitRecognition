using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionDisplay.Model
{
    [Serializable]
    public class InputNode : BaseNode
    {
        public double inputValue { get; set; }
        override public double Activate()
        {
            Activation = inputValue;
            return inputValue;
        }

        public override string ToString()
        {
            return Name + ": " + inputValue;
        }
    }
}
