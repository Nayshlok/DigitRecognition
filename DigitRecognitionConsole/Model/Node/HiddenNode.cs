using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class HiddenNode : ActivatingNode
    {
        public double AdjustWeights()
        {
            double WeightedError = 0;
            foreach (NetConnection nc in Outputs)
            {
                WeightedError += nc.Receiver.Error * nc.Weight;
            }
            double activation = activationFunction();
            double error = activation * (1 - activation) * WeightedError;

            foreach (NetConnection nc in Inputs)
            {
                nc.Weight = nc.Weight + (LEARNING_RATE * error * nc.Sender.Activation);
            }

            return error;
        }

        public override string ToString()
        {
            return Name + ", A: " + Activation + ", E: " + Error;
        }

    }
}
