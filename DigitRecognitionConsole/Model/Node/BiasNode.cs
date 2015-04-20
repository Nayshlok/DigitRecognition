using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    [Serializable]
    public class BiasNode : BaseNode
    {
        private readonly int BIAS_ACTIVATION = 1;
        public override double Activate()
        {
            Activation = BIAS_ACTIVATION;
            return BIAS_ACTIVATION;
        }

        public override string ToString()
        {
            return "Bias Node";
        }
    }
}
