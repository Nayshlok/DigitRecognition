using DigitRecognitionDisplay.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionDisplay.Model
{
    [Serializable]
    public class OutputNode : ActivatingNode
    {
        public double CalculateError(int target)
        {
            this.Error += Activation * (1 - Activation) * (target - Activation);
            return this.Error;
        }

        public override string ToString()
        {
            return Name + ", A: " + Math.Round(Activation, 5) + ", E: " + Math.Round(Error, 7);
        }
    }
}
