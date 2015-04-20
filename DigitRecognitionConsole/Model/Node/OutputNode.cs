using DigitRecognitionConsole.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    [Serializable]
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

        public double CalculateError(int target)
        {
            this.Error += Activation * (1 - Activation) * (target - Activation);
            return this.Error;
        }

        public override string ToString()
        {
            return Name + ", Out Value = " + OutputValue + ", A: " + Math.Round(Activation, 5) + ", E: " + Math.Round(Error, 7);
        }
    }
}
