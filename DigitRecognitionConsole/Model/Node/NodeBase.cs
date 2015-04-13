using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public abstract class NodeBase
    {
        protected static readonly double LEARNING_RATE = 0.2;

        public string Name { get; set; }

        public List<NetConnection> Outputs { get; set; }

        public NodeBase()
        {
            Outputs = new List<NetConnection>();
        }

        public void AddConnection(LayerNode node)
        {
            NetConnection connection = new NetConnection(this, node);
            
            Outputs.Add(connection);
            node.Inputs.Add(connection);
        }

        public abstract double Activate();
    }
}
