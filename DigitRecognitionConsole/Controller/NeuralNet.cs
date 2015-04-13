using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class NeuralNet
    {
        public InputNode[] inputNodes;
        public OutputNode[] outputNodes;
        private BiasNode bias;

        public NeuralNet(int inputs, int[] outputs)
        {
            if (inputs == 0)
            {
                throw new Exception("You must have some inputs");
            }
            if (outputs == null || outputs.Length == 0)
            {
                throw new Exception("You must have outputs");
            }
            bias = new BiasNode();
            inputNodes = new InputNode[inputs];
            LayerNode[] Layer1 = new LayerNode[inputNodes.Length];
            for (int i = 0; i < inputNodes.Length; i++)
            {
                inputNodes[i] = new InputNode { Name="I" + i };
                Layer1[i] = new LayerNode { Name="L" + i };
            }
            Layer1[Layer1.Length - 1] = new LayerNode { Name="L" + (Layer1.Length - 1) };
            outputNodes = new OutputNode[outputs.Length];
            for (int i = 0; i < outputNodes.Length; i++)
            {
                outputNodes[i] = new OutputNode(outputs[i]) { Name = "O" + i };
            }

            EstablishConnections(inputNodes, Layer1);
            EstablishConnections(Layer1, outputNodes);

        }

        private void EstablishConnections(NodeBase[] SendingNodes, LayerNode[] ReceivingNodes)
        {
            foreach (NodeBase sn in SendingNodes)
            {
                foreach (LayerNode rn in ReceivingNodes)
                {
                    sn.AddConnection(rn);
                }
            }
            foreach (LayerNode n in ReceivingNodes)
            {
                bias.AddConnection(n);
            }
        }

        public double judgeInput(byte[] data)
        {
            if (data.Length != inputNodes.Length)
            {
                throw new Exception("The incoming data does not fit the network.");
            }
            bias.Activate();
            for (int i = 0; i < inputNodes.Length; i++)
            {
                inputNodes[i].inputValue = data[i];
                inputNodes[i].Activate();
            }
            NodeBase nextLayer = inputNodes[0];
            while (nextLayer != null)
            {
                nextLayer = ActivateNextLayer(nextLayer);
            }
            OutputNode Selected = outputNodes[0];
            double highestActivation = Selected.Activate();
            foreach (OutputNode n in outputNodes)
            {
                
                double activation = n.Activate();
                if (activation > highestActivation)
                {
                    Selected = n;
                    highestActivation = activation;
                }


            }
            
            return Selected.OutputValue;
        }

        public void TrainNetwork(DataItem Item)
        {
            if (Item.data.Length != inputNodes.Length)
            {
                throw new Exception("The incoming data does not fit the network.");
            }
            bias.Activate();
            for (int i = 0; i < inputNodes.Length; i++)
            {
                inputNodes[i].inputValue = Item.data[i];
                inputNodes[i].Activate();
            }
            NodeBase nextLayer = inputNodes[0];
            while (nextLayer != null)
            {
                nextLayer = ActivateNextLayer(nextLayer);
            }
            foreach (OutputNode n in outputNodes)
            {
                int target = n.OutputValue == Item.expectedResult ? 1 : 0;
                n.AdjustWeights(target);
            }
            NodeBase prevLayer = outputNodes[0];
            while (prevLayer != null)
            {
                prevLayer = AdjustPreviousWeight(prevLayer);
            }

        }

        public NodeBase ActivateNextLayer(NodeBase node)
        {
            foreach (NetConnection nc in node.Outputs)
            {
                if(nc.Receiver.Outputs.Count != 0){
                    nc.Receiver.Activate();
                }
            }
            return node.Outputs.Count == 0 ? null: node.Outputs[0].Receiver;
        }

        public NodeBase AdjustPreviousWeight(NodeBase startNode)
        {
            NodeBase PrevNode = null;
            if (startNode is LayerNode)
            {
                LayerNode currentNode = (LayerNode)startNode;
                foreach (NetConnection nc in currentNode.Inputs)
                {
                    if (nc.Sender is LayerNode)
                    {
                        LayerNode temp = (LayerNode)nc.Sender;
                        temp.AdjustWeights();
                    }
                }
                PrevNode = currentNode.Inputs[0].Sender;
            }

            return PrevNode;
        }

    }
}
