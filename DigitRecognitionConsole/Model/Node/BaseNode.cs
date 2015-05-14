using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    [Serializable]
    public abstract class BaseNode
    {
        public static readonly double LEARNING_RATE = 0.01;

        public string Name { get; set; }

        public List<NetConnection> Outputs { get; set; }
        public double Activation { get; set; }
        public double Error { get; set; }

        public BaseNode()
        {
            Outputs = new List<NetConnection>();
        }

        public void AddConnection(ActivatingNode node)
        {
            NetConnection connection = new NetConnection(this, node);
            
            Outputs.Add(connection);
            node.Inputs.Add(connection);
        }

        public double GetActivation()
        {
            return Activation;
        }

        public abstract double Activate();
    }
}
