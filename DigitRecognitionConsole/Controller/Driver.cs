using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;
using DigitRecognitionConsole.Controller;

namespace DigitRecognitionConsole
{
    public class Driver
    {
        public double TotalAccuracy { get; set; }

        private NeuralNet net = new NeuralNet(null, 1, new int[]{1}, 1);
        private IDataProvider provider;
        private IJudge judge;

        public Driver(IDataProvider provider, IJudge judge)
        {
            this.provider = provider;
            this.judge = judge;
        }

        public IEnumerable<double> EpochTrain(int epochSize, int epochs)
        {
            net = new NeuralNet(judge, provider.GetNumOfInputs(), provider.GetHiddenLayerSizes(), provider.GetPossibleOutputs());
            AccuracyFinder accuracy = new AccuracyFinder();
            AccuracyFinder batchAccuracyFinder = new AccuracyFinder();
            double MSE = 0;

            int Index = 0;
            for (int i = 0; i < epochs; i++)
            {
                if (Index >= provider.GetSetSize())
                {
                    Index = 0;
                }
                double batchAccuracy = 0;
                foreach (DataItem nextItem in provider.GetDataItems().Skip(Index).Take(epochSize))
                {
                    //StoredNetwork.Network.TrainNetwork(nextItem);
                    OutputNode[] networkResults = net.judgeInput(nextItem.data);
                    int[] judgeResults = judge.TrainingResult(nextItem, networkResults);
                    net.TrainNetwork(nextItem);
                }
                Index += epochSize;

                batchAccuracyFinder.TestNetworkAccuracy(net, provider, judge, epochSize);
                MSE = batchAccuracyFinder.TotalAccuracy / 100;
                yield return MSE;
            }

        }

        public Dictionary<int, AccuracyData> TestNetwork(IDataProvider testProvider, int testCount = -1)
        {
            testCount = testCount == -1 ? provider.GetSetSize() : testCount;
            AccuracyFinder tester = new AccuracyFinder();
            Dictionary<int, AccuracyData> testData = tester.TestNetworkAccuracy(net, testProvider, judge, testCount);
            TotalAccuracy = tester.TotalAccuracy;
            return testData;
        }

    }
}
