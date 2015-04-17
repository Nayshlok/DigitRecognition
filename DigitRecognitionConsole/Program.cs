using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;
using DigitRecognitionConsole.Controller;
using System.Diagnostics;

namespace DigitRecognitionConsole
{
    class Program
    {
        private static readonly string TrainingDataPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte";
        private static readonly string TrainingLabelPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte";
        private static readonly string TestingDataPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\t10k-images.idx3-ubyte";
        private static readonly string TestingLabelPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\t10k-labels.idx1-ubyte";
        
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            IDataProvider provider = new DigitDataReader(TrainingDataPath, TrainingLabelPath);
            NeuralNet net2 = new NeuralNet(provider.GetNumOfInputs(), provider.GetHiddenLayerSize(), provider.GetPossibleOutputs());
            watch.Start();
            foreach (DataItem nextItem in provider.GetNextDataItem())
            {
                net2.TrainNetwork(nextItem);
            }
            watch.Stop();
            Console.WriteLine("Milliseconds to process: " + watch.ElapsedMilliseconds);

            watch.Reset();
            watch.Start();
            IDataProvider TestProvider = new DigitDataReader(TestingDataPath, TestingLabelPath);
            Console.WriteLine("Percentage correct: " + TestNetworkAccuracy(net2, provider) + "%");
            watch.Stop();
            Console.WriteLine("Milliseconds to test: " + watch.ElapsedMilliseconds);

            //PrintTestResults(new byte[] { 1, 1 }, 0, net2);
            //PrintTestResults(new byte[] { 1, 0 }, 1, net2);
            //PrintTestResults(new byte[] { 0, 1 }, 1, net2);
            //PrintTestResults(new byte[] { 0, 0 }, 0, net2);
            //PrintAllWeights(net2);

        }

        public static void PrintXORTestResults(byte[] data, int expected, NeuralNet net)
        {
            OutputNode TestResult = net.judgeInput(data);
            net.outputNodes[0].CalculateError(expected);
            net.outputNodes[1].CalculateError(expected);
            Console.WriteLine("Testing " + data[0] + ", " + data[1] + ": received " + TestResult);
            Console.WriteLine(net.outputNodes[0]);
            Console.WriteLine(net.outputNodes[1]);
        }

        public static void PrintAllWeights(NeuralNet net)
        {
            Console.WriteLine();
            foreach (OutputNode n in net.outputNodes)
            {
                foreach (NetConnection nc in n.Inputs)
                {
                    Console.WriteLine(nc);
                }
            }
            foreach (NetConnection n in net.outputNodes[0].Inputs)
            {
                if (n.Sender is ActivatingNode)
                {
                    ActivatingNode an = n.Sender as ActivatingNode;
                    foreach (NetConnection nc in an.Inputs)
                    {
                        Console.WriteLine(nc);
                    }
                }
            }
            foreach (OutputNode o in net.outputNodes)
            {
                Console.WriteLine(o.Name + " A: " + o.Activation);
            }
        }

        public static double TestNetworkAccuracy(NeuralNet net, IDataProvider provider)
        {
            int CorrectCount = 0;
            foreach (DataItem item in provider.GetNextDataItem())
            {
                OutputNode Result = net.judgeInput(item.data);
                if (Result.OutputValue == item.expectedResult)
                {
                    CorrectCount++;
                }
            }
            return (((double)CorrectCount) / ((double)provider.GetSetSize()));
        }

    }
}
/*
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte
 */
