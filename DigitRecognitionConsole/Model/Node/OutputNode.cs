using DigitRecognitionConsole.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class OutputNode : LayerNode
    {
        private int _OutputValue;
        public int OutputValue
        {
            get
            {
                return _OutputValue;
            }
        }

        public OutputNode(int OutputValue)
        {
            _OutputValue = OutputValue;
        }

        public double AdjustWeights(int target)
        {
            double activation = this.activationFunction();
            double error = activation * (1 - activation) * (target - activation);
            foreach (NetConnection nc in Inputs)
            {
                nc.Error = error;
                nc.Weight = nc.Weight + (LEARNING_RATE * error * nc.Activation);
            }
            return error;
        }

        public new double AdjustWeights()
        {
            throw new Exception("Output node has no outputs to adjust weights from. Please provide a target.");
        }
    }
}
