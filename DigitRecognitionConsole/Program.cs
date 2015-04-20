using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;
using DigitRecognitionConsole.Controller;
using System.Diagnostics;
using System.IO;

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
            //IDataProvider provider = new DigitDataReader(TrainingDataPath, TrainingLabelPath);
            IDataProvider provider = new BinaryXORProvider();
            PersistentNetwork StoredNetwork = null;
            Console.WriteLine("Enter File Name:");
            string FileName = Console.ReadLine();
            try
            {
                StoredNetwork = NetworkPersist.LoadNetwork(FileName);
            }
            catch (FileNotFoundException ex)
            {
                StoredNetwork = new PersistentNetwork { Index = 0, Network = new NeuralNet(provider.GetNumOfInputs(), provider.GetHiddenLayerSize(), provider.GetPossibleOutputs()) };
            }

            Console.WriteLine("Current index = " + StoredNetwork.Index);
            Console.WriteLine("Enter number of images to process per batch");
            int batchSize = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter number of batches");
            int batches = int.Parse(Console.ReadLine());
            Stopwatch watch = new Stopwatch();

            for (int i = 0; i < batches; i++)
            {
                watch.Start();
                foreach (DataItem nextItem in provider.GetNextDataItem().Skip(StoredNetwork.Index).Take(batchSize))
                {
                    StoredNetwork.Network.TrainNetwork(nextItem);
                }

                StoredNetwork.Index += batchSize;
                NetworkPersist.SaveNetwork(StoredNetwork, FileName);

                watch.Stop();
                Console.WriteLine("Milliseconds to process: " + watch.ElapsedMilliseconds);
                watch.Reset();
            }

            Console.WriteLine("Test?");
            string response = Console.ReadLine();
            if (response == "y")
            {
                watch.Reset();
                watch.Start();
                //IDataProvider TestProvider = new DigitDataReader(TestingDataPath, TestingLabelPath);
                IDataProvider TestProvider = new BinaryXORProvider();
                Console.WriteLine("Percentage correct: " + TestNetworkAccuracy(StoredNetwork.Network, TestProvider, 10000) + "%");
                watch.Stop();
                Console.WriteLine("Milliseconds to test: " + watch.ElapsedMilliseconds);

            }

            PrintXORTestResults(new byte[] { 1, 1 }, 0, StoredNetwork.Network);
            PrintXORTestResults(new byte[] { 1, 0 }, 1, StoredNetwork.Network);
            PrintXORTestResults(new byte[] { 0, 1 }, 1, StoredNetwork.Network);
            PrintXORTestResults(new byte[] { 0, 0 }, 0, StoredNetwork.Network);
            PrintAllWeights(StoredNetwork.Network);

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

        public static double TestNetworkAccuracy(NeuralNet net, IDataProvider provider, int testNumber = -1)
        {
            int setSize = testNumber == -1 ? provider.GetSetSize() : testNumber;
            int CorrectCount = 0;
            foreach (DataItem item in provider.GetNextDataItem().Take(setSize))
            {
                OutputNode Result = net.judgeInput(item.data);
                if (Result.OutputValue == item.expectedResult)
                {
                    CorrectCount++;
                }
            }
            return (((double)CorrectCount) / ((double)setSize)) * 100;
        }

    }
}
/*
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte
 */
