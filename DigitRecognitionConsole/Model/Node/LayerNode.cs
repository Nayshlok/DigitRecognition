using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class LayerNode : NodeBase
    {
        public List<NetConnection> Inputs { get; set; }

        public LayerNode()
        {
            Inputs = new List<NetConnection>();
        }

        override public double Activate()
        {
            if (Inputs == null || Inputs.Count == 0)
            {
                throw new Exception("There are no inputs");
            }
            double activation = activationFunction();
            if (Outputs.Count != 0)
            {
                foreach (NetConnection n in Outputs)
                {
                    n.Activation = activation;
                }
            }
            return activation;
        }

        public double activationFunction()
        {
            double activation = 0;
            double SumOfWeightedActivation = 0;
            foreach (NetConnection nc in Inputs)
            {
                SumOfWeightedActivation += nc.Activation * nc.Weight;
            }

            activation = 1 / (1 + Math.Pow(Math.E, -1 * SumOfWeightedActivation));

            return activation;
        }

        public double AdjustWeights()
        {
            double WeightedError = 0;
            foreach (NetConnection nc in Outputs)
            {
                WeightedError += nc.Error * nc.Weight;
            }
            double activation = activationFunction();
            double error = activation * (1 - activation) * WeightedError;

            foreach (NetConnection nc in Inputs)
            {
                nc.Weight = nc.Weight + (LEARNING_RATE * error * nc.Activation);
            }

            return error;
        }

    }
}
