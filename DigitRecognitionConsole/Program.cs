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

        private static double TotalAccuracy;

        static void Main(string[] args)
        {
            IDataProvider provider = new DigitProvider(TrainingDataPath, TrainingLabelPath);
            //IDataProvider provider = new BitAdditionProvider();
            PersistentNetwork StoredNetwork = null;
            Console.WriteLine("Enter File Name:");
            string FileName = Console.ReadLine();
            try
            {
                StoredNetwork = NetworkPersist.LoadNetwork(FileName);
            }
            catch (FileNotFoundException)
            {
                StoredNetwork = new PersistentNetwork { Index = 0, Network = new NeuralNet(provider.GetNumOfInputs(), provider.GetHiddenLayerSizes(), provider.GetPossibleOutputs()) };
            }
            IDataProvider TestProvider = new DigitProvider(TestingDataPath, TestingLabelPath);
            //IDataProvider TestProvider = new BitAdditionProvider();

            IJudge judge = new DigitJudge();

            Console.WriteLine("Current index = " + StoredNetwork.Index);
            Dictionary<int, AccuracyData> indivdualData = TestNetworkAccuracy(StoredNetwork.Network, TestProvider, judge, 1000);
            Console.WriteLine("Percentage correct: " + TotalAccuracy + "%");
            foreach (KeyValuePair<int, AccuracyData> kv in indivdualData)
            {
                Console.WriteLine(kv.Key + ": " + kv.Value.correct + " correct of " + kv.Value.total);
            }

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
                    //StoredNetwork.Network.TrainNetwork(nextItem);
                    OutputNode[] networkResults = StoredNetwork.Network.judgeInput(nextItem.data);
                    bool[] judgeResults = judge.TrainingResult(nextItem, networkResults);
                    StoredNetwork.Network.TrainNetwork(judgeResults);
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
                indivdualData = TestNetworkAccuracy(StoredNetwork.Network, TestProvider, judge, 1000);
                Console.WriteLine("Percentage correct: " + TotalAccuracy + "%");
                foreach (KeyValuePair<int, AccuracyData> kv in indivdualData)
                {
                    Console.WriteLine(kv.Key + ": " + kv.Value.correct + " correct of " + kv.Value.total);
                }
                watch.Stop();
                Console.WriteLine("Milliseconds to test: " + watch.ElapsedMilliseconds);

            }
            //PrintAllWeights(StoredNetwork.Network);

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

        public static Dictionary<int, AccuracyData> TestNetworkAccuracy(NeuralNet net, IDataProvider provider, IJudge judge, int testNumber = -1)
        {
            int setSize = testNumber == -1 ? provider.GetSetSize() : testNumber;
            int CorrectCount = 0;
            int index = 0;
            judge.ResetTraining();
                foreach (DataItem item in provider.GetNextDataItem().Take(setSize))
                {
                    OutputNode[] Result = net.judgeInput(item.data);
                    bool test = judge.JudgeNetwork(item, Result);
                    if (test)
                    {
                        CorrectCount++;
                    }
                    index++;
                }
            TotalAccuracy = (((double)CorrectCount) / ((double)setSize)) * 100;
            return judge.getAccuracy();
        }
    }
}
/*
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-images.idx3-ubyte
C:\Users\David Borland\Documents\Capstone\DigitRecognitionConsole\DigitRecognitionConsole\Data\train-labels.idx1-ubyte
 */
