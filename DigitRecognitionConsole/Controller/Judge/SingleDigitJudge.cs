using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    [Serializable]
    public class SingleDigitJudge : IJudge
    {

        private readonly int OUTPUT_SIZE = 1;
        private readonly double INACTIVE_THRESHOLD = 0.25;
        private readonly double ACTIVE_THRESHOLD = 0.75;
        private int ExpectedValue;

        public SingleDigitJudge(int expected)
        {
            this.ExpectedValue = expected;
        }

        public bool JudgeNetwork(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            return (ShouldBeZero(outputs[0].Activation, Item.expectedResult) || ShouldBeOne(outputs[0].Activation, Item.expectedResult));
        }

        private bool ShouldBeZero(double activation, int expected)
        {
            return activation < INACTIVE_THRESHOLD && expected != ExpectedValue;
        }

        private bool ShouldBeOne(double activation, int expected)
        {
            return activation > ACTIVE_THRESHOLD && expected == ExpectedValue;
        }

        public int[] TrainingResult(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            int[] shouldActivate = new int[outputs.Length];
            shouldActivate[0] = Item.expectedResult == ExpectedValue ? 1 : 0;
            return shouldActivate;
        }

        public Dictionary<int, AccuracyData> getEmptyAccuracyInfo()
        {
            Dictionary<int, AccuracyData> _Accuracy = new Dictionary<int, AccuracyData>();
            for (int i = 0; i < 10; i++)
            {
                _Accuracy[i] = new AccuracyData();
            }
            return _Accuracy;
        }

        private void TryValidParamters(OutputNode[] outputs)
        {
            if (outputs.Length != OUTPUT_SIZE)
            {
                throw new Exception("Output size does not match this judge.");
            }
        }
    }
}
