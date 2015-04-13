using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public class NetConnection
    {
        public string Name { get; set; }

        private static Random rand = new Random();
        public NodeBase Sender { get; set; }
        public NodeBase Receiver { get; set; }
        public double Weight { get; set; }
        public double Activation { get; set; }
        public double Error { get; set; }

        public NetConnection(NodeBase Sender, NodeBase Receiver)
        {
            this.Sender = Sender;
            this.Receiver = Receiver;
            Weight = rand.NextDouble();
            //Weight = 0;
        }

        public override string ToString()
        {
            return Sender.Name + " to " + Receiver.Name + "; W = " + Math.Round(Weight, 7) + ", A = " + Math.Round(Activation, 7) + ", E = " + Math.Round(Error, 7);
        }

    }
}
