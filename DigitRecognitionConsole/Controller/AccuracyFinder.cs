using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class AccuracyFinder
    {
        public double TotalAccuracy { get; private set; }

        public Dictionary<int, AccuracyData> TestNetworkAccuracy(NeuralNet net, IDataProvider provider, IJudge judge, int testNumber = -1)
        {
            int setSize = testNumber == -1 ? provider.GetSetSize() : testNumber;
            int CorrectCount = 0;
            Dictionary<int, AccuracyData> accuracyInfo = judge.getEmptyAccuracyInfo();
            foreach (DataItem item in provider.GetNextDataItem().Take(setSize))
            {
                OutputNode[] Result = net.judgeInput(item.data);
                bool test = judge.JudgeNetwork(item, Result);
                accuracyInfo[item.expectedResult].total++;
                if (test)
                {
                    CorrectCount++;
                    accuracyInfo[item.expectedResult].correct++;
                }
            }
            TotalAccuracy = (((double)CorrectCount) / ((double)setSize)) * 100;
            return accuracyInfo;
        }

        public double CalculateSumSquareError(IJudge judge, OutputNode[] nodes, DataItem Item)
        {
            double error = 0;
            int[] resultSet = judge.TrainingResult(Item, nodes);
            for (int i = 0; i < nodes.Length; i++)
            {
                error += Math.Pow((resultSet[i] - nodes[i].Activation), 2);
            }
            return error;
        }
    }
}
