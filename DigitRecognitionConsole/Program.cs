using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;
using DigitRecognitionConsole.Controller;

namespace DigitRecognitionConsole
{
    class Program
    {
        //private static readonly string DataPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte";
        //private static readonly string LabelPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte";
        
        static void Main(string[] args)
        {
            IDataProvider reader = new BinaryXORProvider();
            NeuralNet net = new NeuralNet(reader.GetNumOfInputs(), reader.GetPossibleOutputs());

            //DataItem nextItem = reader.GetNextDataItem();
            //Console.WriteLine(net.judgeInput(nextItem) + ", expected " + nextItem.expectedResult);

            //PrintAllWeights(net);

            //int correctCount = 0;
            DataItem nextItem;
            //for (int i = 0; i < reader.GetTrainingSetSize(); i++)
            //{
            //    nextItem = new DataItem { data = new byte[] { 0, 1 }, expectedResult = 0 ^ 1 };
            //    if (net.judgeInput(nextItem.data) == nextItem.expectedResult)
            //    {
            //        correctCount++;
            //    }
            //}
            //Console.WriteLine("The network got " + correctCount + " correct of " + reader.GetTrainingSetSize());

            //for (int i = 0; i < reader.GetTrainingSetSize(); i++)
            //{
            //    nextItem = new DataItem { data = new byte[] { 0, 1 }, expectedResult = 0 ^ 1 };
            //    net.TrainNetwork(nextItem);
            //}

            //correctCount = 0;
            //for (int i = 0; i < reader.GetTrainingSetSize(); i++)
            //{
            //    nextItem = new DataItem { data = new byte[] { 0, 1 }, expectedResult = 0 ^ 1 };
            //    if (net.judgeInput(nextItem.data) == nextItem.expectedResult)
            //    {
            //        correctCount++;
            //    }
            //}
            //Console.WriteLine("The network got " + correctCount + " correct of " + reader.GetTrainingSetSize());

            //PrintAllWeights(net);

            NeuralNet net2 = new NeuralNet(reader.GetNumOfInputs(), reader.GetPossibleOutputs());
            DataItem[] possibles = new DataItem[] {
                new DataItem { data = new byte[] { 1, 1 }, expectedResult = 1 ^ 1 },
                new DataItem { data = new byte[] { 1, 0 }, expectedResult = 1 ^ 0 },
                new DataItem { data = new byte[] { 0, 1 }, expectedResult = 0 ^ 1 },
                new DataItem { data = new byte[] { 0, 0 }, expectedResult = 0 ^ 0 }
            };
            int pointer = 0;
            for (int i = 0; i < reader.GetTrainingSetSize(); i++)
            {
                nextItem = possibles[pointer];
                net2.TrainNetwork(nextItem);
                pointer = pointer >= possibles.Length - 1 ? 0 : pointer + 1;
            }
            PrintAllWeights(net2);
            Console.WriteLine("Testing 1, 1 received " + net2.judgeInput(new byte[] { 1, 1}));
            Console.WriteLine("Testing 0, 1 received " + net2.judgeInput(new byte[] { 0, 1 }));
            Console.WriteLine("Testing 1, 0 received " + net2.judgeInput(new byte[] { 1, 0 }));
            Console.WriteLine("Testing 0, 0 received " + net2.judgeInput(new byte[] { 0, 0 }));
            PrintAllWeights(net2);

        }

        public static void PrintAllWeights(NeuralNet net)
        {
            Console.WriteLine();
            foreach (NodeBase n in net.inputNodes)
            {
                foreach (NetConnection nc in n.Outputs)
                {
                    Console.WriteLine(nc);
                }
            }
            foreach (NetConnection n in net.inputNodes[0].Outputs)
            {
                foreach (NetConnection nc in n.Receiver.Outputs)
                {
                    Console.WriteLine(nc);
                }
            }
            foreach (OutputNode o in net.outputNodes)
            {
                Console.WriteLine(o.Name + " A: " + o.Activate());
            }
        }
    }
}
/*
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte
 */
