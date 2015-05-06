using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;
using DigitRecognitionConsole.Controller;

namespace DigitRecognitionDisplay
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
            double MSE = 0;

            int Index = 0;
            for (int i = 0; i < epochs; i++)
            {
                if (Index >= provider.GetSetSize())
                {
                    Index = 0;
                }
                double batchError = 0;
                foreach (DataItem nextItem in provider.GetNextDataItem().Skip(Index).Take(epochSize))
                {
                    //StoredNetwork.Network.TrainNetwork(nextItem);
                    OutputNode[] networkResults = net.judgeInput(nextItem.data);
                    batchError += accuracy.CalculateSumSquareError(judge, networkResults, nextItem);
                    int[] judgeResults = judge.TrainingResult(nextItem, networkResults);
                    net.TrainNetwork(nextItem);
                }

                Index += epochSize;

                MSE = batchError / (epochSize + provider.GetPossibleOutputs());

                yield return MSE;
            }

        }

        public Dictionary<int, AccuracyData> TestNetwork(IDataProvider testProvider, int testCount)
        {
            AccuracyFinder tester = new AccuracyFinder();
            Dictionary<int, AccuracyData> testData = tester.TestNetworkAccuracy(net, testProvider, judge, testCount);
            TotalAccuracy = tester.TotalAccuracy;
            return testData;
        }

    }
}
