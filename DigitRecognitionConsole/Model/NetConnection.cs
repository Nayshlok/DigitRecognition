using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionDisplay.Model
{
    [Serializable]
    public class NetConnection
    {
        public string Name { get; set; }

        private static Random rand = new Random();
        public BaseNode Sender { get; set; }
        public BaseNode Receiver { get; set; }
        public double Weight { get; set; }
        //public double Activation { get; set; }
        //public double Error { get; set; }

        public NetConnection(BaseNode Sender, BaseNode Receiver)
        {
            this.Sender = Sender;
            this.Receiver = Receiver;
            Weight = rand.NextDouble() - 0.5;
            //Weight = 0;
        }

        public override string ToString()
        
        {
            return Sender.Name + " to " + Receiver.Name + "; W = " + Math.Round(Weight, 7);
        }

    }
}
