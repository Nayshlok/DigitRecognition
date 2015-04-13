using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class InputNode : NodeBase
    {
        public double inputValue { get; set; }
        override public double Activate()
        {
            foreach (NetConnection n in Outputs)
            {
                n.Activation = inputValue;
            }
            return inputValue;
        }
    }
}
