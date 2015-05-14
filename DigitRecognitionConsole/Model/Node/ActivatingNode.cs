using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    [Serializable]
    public abstract class ActivatingNode : BaseNode
    {
        private readonly double ERROR_MAXIMUM = 0.148148;
        private readonly double Momentum = 0.5;
        public List<NetConnection> Inputs { get; set; }

        public ActivatingNode()
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
            this.Activation = activation;
            return activation;
        }

        public double activationFunction()
        {
            double activation = 0;
            double SumOfWeightedActivation = 0;
            foreach (NetConnection nc in Inputs)
            {
                SumOfWeightedActivation += nc.Sender.Activation * nc.Weight;
            }

            //activation = SumOfWeightedActivation;
            activation = 1 / (1 + Math.Exp(-1 * SumOfWeightedActivation));

            return activation;
        }

        public void AdjustWeights()
        {
            foreach (NetConnection nc in Inputs)
            {
                //nc.Weight = nc.Weight + (LEARNING_RATE * Error * nc.Sender.Activation);// Original no momentum
                //nc.Weight = nc.Weight + ((Math.Abs(Error / ERROR_MAXIMUM) * LEARNING_RATE) * Error * nc.Sender.Activation); //My attempt at momentum that uses the relative error as it's learning rate.
                nc.Weight = nc.Weight + (LEARNING_RATE * Error * nc.Sender.Activation) + (Momentum * nc.PreviousChange);// adding a fraction of previous change
            }
            this.Error = 0;
        }
    }
}
