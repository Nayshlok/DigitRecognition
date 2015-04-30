using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    [Serializable]
    public class NeuralNet
    {
        private InputNode[] inputNodes;
        public OutputNode[] outputNodes;
        private BiasNode bias;

        public NeuralNet(int inputs, int[] HiddenLayerSizes, int outputs)
        {
            if (inputs == 0)
            {
                throw new Exception("You must have some inputs");
            }
            if (outputs== 0)
            {
                throw new Exception("You must have outputs");
            }
            bias = new BiasNode();
            bias.Name = "B";
            inputNodes = new InputNode[inputs];
            //HiddenNode[] Layer1 = new HiddenNode[HiddenLayerSizes[0]];
            for (int i = 0; i < inputNodes.Length; i++)
            {
                inputNodes[i] = new InputNode { Name="I" + i };
            }
            //for (int i = 0; i < Layer1.Length; i++)
            //{
            //    Layer1[i] = new HiddenNode { Name = "L" + i };
            //}
            outputNodes = new OutputNode[outputs];
            for (int i = 0; i < outputNodes.Length; i++)
            {
                outputNodes[i] = new OutputNode() { Name = "O" + i };
            }

            BaseNode[] PreviousLayer = inputNodes;
            for (int i = 0; i < HiddenLayerSizes.Length; i++)
            {
                HiddenNode[] HiddenLayer = new HiddenNode[HiddenLayerSizes[i]];
                for (int j = 0; j < HiddenLayer.Length; j++)
                {
                    HiddenLayer[j] = new HiddenNode { Name = "L(" + i + "," + j + ")"};
                }
                EstablishConnections(PreviousLayer, HiddenLayer);
                PreviousLayer = HiddenLayer;
            }
            EstablishConnections(PreviousLayer, outputNodes);
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

        public OutputNode[] judgeInput(double[] data)
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
            return outputNodes;
        }

        public void TrainNetwork(bool[] shouldActivate)
        {
            for (int i = 0; i < outputNodes.Length; i++)
            {
                int target = shouldActivate[i] ? 1 : 0;
                outputNodes[i].CalculateError(target);
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

        //public void BatchTrainNetwork(IEnumerable<DataItem> items)
        //{
        //    double error = 0;
        //    foreach (DataItem Item in items)
        //    {
        //        if (Item.data.Length != inputNodes.Length)
        //        {
        //            throw new Exception("The incoming data does not fit the network.");
        //        }
        //        bias.Activate();
        //        for (int i = 0; i < inputNodes.Length; i++)
        //        {
        //            inputNodes[i].inputValue = Item.data[i];
        //            inputNodes[i].Activate();
        //        }
        //        BaseNode nextLayer = inputNodes[0];
        //        while (nextLayer != null)
        //        {
        //            nextLayer = ActivateNextLayer(nextLayer);
        //        }
        //        foreach (OutputNode n in outputNodes)
        //        {
        //            int target = n.OutputValue == Item.expectedResult ? 1 : 0;
        //            n.CalculateError(target);
        //        }
        //        BaseNode prevLayer = outputNodes[0];
        //        while (prevLayer != null)
        //        {
        //            prevLayer = CalculatePreviousError(prevLayer);
        //        }
        //    }

        //    foreach (OutputNode n in outputNodes)
        //    {
        //        n.AdjustWeights();
        //    }
        //    BaseNode previous = outputNodes[0];
        //    while (previous != null)
        //    {
        //        previous = AdjustPreviousWeight(previous);
        //    }
        //}

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
                        //temp.AdjustWeights();
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
