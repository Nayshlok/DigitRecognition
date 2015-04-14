using DigitRecognitionConsole.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class OutputNode : ActivatingNode
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
            foreach (NetConnection nc in Inputs)
            {
                nc.Weight = nc.Weight + (LEARNING_RATE * Error * nc.Sender.Activation);
            }
            return Error;
        }

        public double CalculateError(int target)
        {
            this.Error = Activation * (1 - Activation) * (target - Activation);
            return this.Error;
        }

        public override string ToString()
        {
            return Name + ", Out Value = " + OutputValue + ", A: " + Activation + ", E: " + Error;
        }
    }
}
