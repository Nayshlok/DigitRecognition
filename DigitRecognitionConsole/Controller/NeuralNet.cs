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

        public NeuralNet(int inputs, int HiddenLayersize, int[] outputs)
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
            bias.Name = "B";
            inputNodes = new InputNode[inputs];
            HiddenNode[] Layer1 = new HiddenNode[HiddenLayersize];
            for (int i = 0; i < inputNodes.Length; i++)
            {
                inputNodes[i] = new InputNode { Name="I" + i };
            }
            for (int i = 0; i < Layer1.Length; i++)
            {
                Layer1[i] = new HiddenNode { Name = "L" + i };
            }
            outputNodes = new OutputNode[outputs.Length];
            for (int i = 0; i < outputNodes.Length; i++)
            {
                outputNodes[i] = new OutputNode(outputs[i]) { Name = "O" + i };
            }

            EstablishConnections(inputNodes, Layer1);
            EstablishConnections(Layer1, outputNodes);

        }

        private void EstablishConnections(BaseNode[] SendingNodes, ActivatingNode[] ReceivingNodes)
        {
            foreach (BaseNode sn in SendingNodes)
            {
                foreach (ActivatingNode rn in ReceivingNodes)
                {
                    sn.AddConnection(rn);
                }
            }
            foreach (ActivatingNode n in ReceivingNodes)
            {
                bias.AddConnection(n);
            }
        }

        public OutputNode judgeInput(byte[] data)
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
            BaseNode nextLayer = inputNodes[0];
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
            return Selected;
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
            BaseNode nextLayer = inputNodes[0];
            while (nextLayer != null)
            {
                nextLayer = ActivateNextLayer(nextLayer);
            }
            foreach (OutputNode n in outputNodes)
            {
                int target = n.OutputValue == Item.expectedResult ? 1 : 0;
                n.CalculateError(target);
            }
            BaseNode prevLayer = outputNodes[0];
            while (prevLayer != null)
            {
                prevLayer = CalculatePreviousError(prevLayer);
            }

            foreach (OutputNode n in outputNodes)
            {
                n.AdjustWeights();
            }
            prevLayer = outputNodes[0];
            while (prevLayer != null)
            {
                prevLayer = AdjustPreviousWeight(prevLayer);
            }
        }

        public BaseNode ActivateNextLayer(BaseNode node)
        {
            foreach (NetConnection nc in node.Outputs)
            {
                nc.Receiver.Activate();
            }
            return node.Outputs.Count == 0 ? null: node.Outputs[0].Receiver;
        }

        public BaseNode CalculatePreviousError(BaseNode startNode)
        {
            BaseNode PrevNode = null;
            if (startNode is ActivatingNode)
            {
                ActivatingNode currentNode = (ActivatingNode)startNode;
                foreach (NetConnection nc in currentNode.Inputs)
                {
                    if (nc.Sender is HiddenNode)
                    {
                        HiddenNode temp = (HiddenNode)nc.Sender;
                        temp.CalculateError();
                    }
                }
                PrevNode = currentNode.Inputs[0].Sender;
            }

            return PrevNode;
        }

        public BaseNode AdjustPreviousWeight(BaseNode startNode)
        {
            BaseNode PrevNode = null;
            if (startNode is ActivatingNode)
            {
                ActivatingNode currentNode = (ActivatingNode)startNode;
                foreach (NetConnection nc in currentNode.Inputs)
                {
                    if (nc.Sender is ActivatingNode)
                    {
                        ActivatingNode temp = (ActivatingNode)nc.Sender;
                        temp.AdjustWeights();
                    }
                }
                PrevNode = currentNode.Inputs[0].Sender;
            }

            return PrevNode;
        }

    }
}
