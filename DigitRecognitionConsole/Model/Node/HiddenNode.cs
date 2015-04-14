using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class HiddenNode : ActivatingNode
    {


        public double CalculateError()
        {
            double WeightedError = 0;
            foreach (NetConnection nc in Outputs)
            {
                WeightedError += nc.Receiver.Error * nc.Weight;
            }
            double activation = activationFunction();
            this.Error = activation * (1 - activation) * WeightedError;
            return Error;
        }

        public override string ToString()
        {
            return Name + ", A: " + Activation + ", E: " + Error;
        }

    }
}
