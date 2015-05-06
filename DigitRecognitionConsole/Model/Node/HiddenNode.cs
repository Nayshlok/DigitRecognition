using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    [Serializable]
    public class HiddenNode : ActivatingNode
    {
        public double CalculateError()
        {
            double WeightedError = 0;
            foreach (NetConnection nc in Outputs)
            {
                WeightedError += nc.Receiver.Error * nc.Weight;
            }
            this.Error += Activation * (1 - Activation) * WeightedError;
            return Error;
        }

        public override string ToString()
        {
            return Name + ", A: " + Activation + ", E: " + Error;
        }

    }
}
