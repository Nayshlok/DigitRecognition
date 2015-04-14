﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Model
{
    public abstract class BaseNode
    {
        protected static readonly double LEARNING_RATE = 2;

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