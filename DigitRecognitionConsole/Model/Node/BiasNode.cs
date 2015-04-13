using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class BiasNode : NodeBase
    {
        private readonly int BIAS_ACTIVATION = 1;
        public override double Activate()
        {
            foreach (NetConnection n in Outputs)
            {
                n.Activation = BIAS_ACTIVATION;
            }
            return BIAS_ACTIVATION;
        }
    }
}
