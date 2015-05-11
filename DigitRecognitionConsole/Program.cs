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
    public class Program
    {
        public static readonly string TrainingDataPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte";
        public static readonly string TrainingLabelPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte";
        public static readonly string TestingDataPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\t10k-images.idx3-ubyte";
        public static readonly string TestingLabelPath = @"C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\t10k-labels.idx1-ubyte";

        static void Main(string[] args)
        {
            AccuracyFinder accuracy = new AccuracyFinder();
            //IDataProvider provider = new DigitProvider(TrainingDataPath, TrainingLabelPath);
            IDataProvider provider = new BitAdditionProvider();
            //IDataProvider TestProvider = new DigitProvider(TestingDataPath, TestingLabelPath);
            IDataProvider TestProvider = new BitAdditionProvider();

            IJudge judge = new BitAdditionJudge();

            PersistentNetwork StoredNetwork = null;
            Console.WriteLine("Enter File Name:");
            string FileName = Console.ReadLine();
            try
            {
                StoredNetwork = NetworkPersist.LoadNetwork(FileName);
            }
            catch (FileNotFoundException)
            {
                StoredNetwork = new PersistentNetwork { Index = 0, Network = new NeuralNet(judge, provider.GetNumOfInputs(), provider.GetHiddenLayerSizes(), provider.GetPossibleOutputs()) };
            }

            Console.WriteLine("Current index = " + StoredNetwork.Index);
            PrintAccuracy(accuracy, accuracy.TestNetworkAccuracy(StoredNetwork.Network, TestProvider, judge, 100));

            NetworkPersist.SaveNetwork(StoredNetwork, FileName);

            //for (int i = 0; i < 10; i++)
            //{
            //    double[] data = SelectData();
            //    Console.WriteLine("enter expected");
            //    int expected = Console.Read() - 48;
            //    Console.ReadLine();
            //    StoredNetwork.Network.TrainNetwork(new DataItem{ data = data, expectedResult = expected});
            //}

            Console.WriteLine("Enter number of images to process per batch");
            int batchSize = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter number of batches");
            int batches = int.Parse(Console.ReadLine());
            //PrintAllWeights(StoredNetwork.Network);
            Stopwatch watch = new Stopwatch();

            for (int i = 0; i < batches; i++)
            {
                watch.Start();
                if (StoredNetwork.Index >= provider.GetSetSize())
                {
                    StoredNetwork.Index = 0;
                }
                foreach (DataItem nextItem in provider.GetNextDataItem().Skip(StoredNetwork.Index).Take(batchSize))
                {
                    StoredNetwork.Network.TrainNetwork(nextItem);
                }
                //StoredNetwork.Network.BatchTrainNetwork(provider.GetNextDataItem().Skip(StoredNetwork.Index).Take(batchSize));

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
                PrintAccuracy(accuracy, accuracy.TestNetworkAccuracy(StoredNetwork.Network, TestProvider, judge));

                watch.Stop();
                Console.WriteLine("Milliseconds to test: " + watch.ElapsedMilliseconds);

            }
            //PrintAllWeights(StoredNetwork.Network);

        }

        static void manyNeuralNetworks()
        {
            AccuracyFinder accuracy = new AccuracyFinder();
            IDataProvider provider = new DigitProvider(TrainingDataPath, TrainingLabelPath);
            //IDataProvider provider = new BinaryXORProvider();

            IDataProvider TestProvider = new DigitProvider(TestingDataPath, TestingLabelPath);
            //IDataProvider TestProvider = new BinaryXORProvider();

            IJudge judge = new DigitJudge();

            NeuralNet[] networks = new NeuralNet[10];
            for (int i = 0; i < networks.Length; i++)
            {
                IJudge indiviualJudge = new SingleDigitJudge(i);
                networks[i] = new NeuralNet(indiviualJudge, provider.GetNumOfInputs(), provider.GetHiddenLayerSizes(), 1);
            }

            foreach (DataItem nextItem in provider.GetNextDataItem().Take(100))
            {
                
            }
        }

        static double[] SelectData()
        {
            Console.WriteLine("Enter two numbers");
            string valueString = Console.ReadLine();
            string[] splitValues = valueString.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
            double[] values = new double[splitValues.Length];
            for (int i = 0; i < values.Length; i++)
            {
                double.TryParse(splitValues[i], out values[i]);
            }
            return values;
        }

        static void PrintAccuracy(AccuracyFinder accuracy, Dictionary<int, AccuracyData> individualData)
        {
                Console.WriteLine("Percentage correct: " + accuracy.TotalAccuracy + "%");
                foreach (KeyValuePair<int, AccuracyData> kv in individualData)
                {
                    Console.WriteLine(kv.Key + ": " + kv.Value.ToString());
                }
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
            ActivatingNode previous = net.outputNodes[0];
            while (previous != null)
            {
                foreach (NetConnection n in previous.Inputs)
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
                previous = previous.Inputs[0].Sender as ActivatingNode;
            }
        }

    }
}
/*
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte
 */
