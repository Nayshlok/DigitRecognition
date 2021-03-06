﻿using System;
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
    public class MultinetProgram
    {

        private JudgedNetwork[] nets = new JudgedNetwork[10];
        private IDataProvider provider = new DigitProvider(Program.TrainingDataPath, Program.TrainingLabelPath);
        private IDataProvider TestProvider = new DigitProvider(Program.TestingDataPath, Program.TestingLabelPath);
        private IDataProvider TestProvider2 = new DigitProvider(Program.TrainingDataPath, Program.TrainingLabelPath);
        private AccuracyFinder accuracy = new AccuracyFinder();

        public void Run()
        {

            Console.WriteLine("Enter File Name:");
            string FileName = Console.ReadLine();
            try
            {
                for (int i = 0; i < nets.Length; i++)
                {
                    nets[i] = new JudgedNetwork();
                    nets[i].Judge = new SingleDigitJudge(i);
                    nets[i].net = NetworkPersist.LoadNetwork(FileName + i);
                }
            }
            catch (FileNotFoundException)
            {
                for (int i = 0; i < nets.Length; i++)
                {
                    IJudge judge = new SingleDigitJudge(i);
                    PersistentNetwork StoredNetwork = new PersistentNetwork { Index = 0, Network = new NeuralNet(judge, provider.GetNumOfInputs(), provider.GetHiddenLayerSizes(), 1) };
                    nets[i] = new JudgedNetwork(StoredNetwork, judge);
                }
            }

            Console.WriteLine("Enter number of images to process per batch");
            int batchSize = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter number of batches");
            int batches = int.Parse(Console.ReadLine());
            Stopwatch watch = new Stopwatch();

                int endBatchSize = batchSize % provider.GetSetSize();
                batches += batchSize / provider.GetSetSize();
                for (int i = 0; i < batches; i++)
                {

                    for (int j = 0; j < nets.Length; j++)
                    {
                        watch.Start();
                        if (nets[j].net.Index >= provider.GetSetSize())
                        {
                            nets[j].net.Index = 0;
                        }
                        foreach (DataItem nextItem in provider.GetDataItems().Skip(nets[j].net.Index).Take(i == batches - 1 ? endBatchSize : batchSize))
                        {
                            nets[j].net.Network.TrainNetwork(nextItem);
                        }
                        //StoredNetwork.Network.BatchTrainNetwork(provider.GetNextDataItem().Skip(StoredNetwork.Index).Take(batchSize));

                        nets[j].net.Index += batchSize;
                        NetworkPersist.SaveNetwork(nets[j].net, FileName + j);
                        watch.Stop();
                        Console.WriteLine("Milliseconds to process: " + watch.ElapsedMilliseconds);
                        watch.Reset();
                    }
                    using (StreamWriter writer = new StreamWriter(@"..\..\Data\MultiNetworkRecord.txt", true))
                    {
                        Accuracies results = TotalAccuracy();
                        writer.Write(i + ", " + results.TestAccuracies);
                        Console.WriteLine("standard test done");
                        writer.WriteLine(", " + results.TrainingAccuracy);
                        Console.WriteLine("Training test done");
                    }
                }

            //Console.WriteLine("Test?");
            //string response = Console.ReadLine();
            //if (response == "y")
            //{
            //    for (int i = 0; i < nets.Length; i++)
            //    {
            //        watch.Reset();
            //        watch.Start();
            //        PrintAccuracy(accuracy, accuracy.TestNetworkAccuracy(nets[i].net.Network, TestProvider, nets[i].Judge));

            //        watch.Stop();
            //        Console.WriteLine("Milliseconds to test: " + watch.ElapsedMilliseconds);
            //    }
            //}

        }

        private Accuracies TotalAccuracy()
        {

            Stopwatch watch = new Stopwatch();
            double testAccuracy = 0;
            double trainingAccuracy = 0;
            for (int i = 0; i < nets.Length; i++)
            {
                watch.Start();
                accuracy.TestNetworkAccuracy(nets[i].net.Network, TestProvider, nets[i].Judge);
                testAccuracy += accuracy.TotalAccuracy;
                accuracy.TestNetworkAccuracy(nets[i].net.Network, TestProvider2, nets[i].Judge, 10000);
                trainingAccuracy += accuracy.TotalAccuracy;
                watch.Stop();
                Console.WriteLine("Milliseconds to process test: " + watch.ElapsedMilliseconds);
                watch.Reset();
            }
            return new Accuracies(testAccuracy, trainingAccuracy);
        }

        void PrintAccuracy(AccuracyFinder accuracy, Dictionary<int, AccuracyData> individualData)
        {
            Console.WriteLine("Percentage correct: " + accuracy.TotalAccuracy + "%");
            foreach (KeyValuePair<int, AccuracyData> kv in individualData)
            {
                Console.WriteLine(kv.Key + ": " + kv.Value.ToString());
            }
        }

        private class Accuracies
        {
            public double TestAccuracies { get; set; }
            public double TrainingAccuracy { get; set; }

            public Accuracies(double TestAccuracy, double TrainingAccuracy)
            {
                this.TestAccuracies = TestAccuracy;
                this.TrainingAccuracy = TrainingAccuracy;
            }
        }

        private class JudgedNetwork
        {
            public IJudge Judge { get; set; }
            public PersistentNetwork net { get; set; }

            public JudgedNetwork()
            {

            }

            public JudgedNetwork(PersistentNetwork net, IJudge judge)
            {
                this.net = net;
                this.Judge = judge;
            }
        }
    }
}
