using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    [Serializable]
    public class XORJudge : IJudge
    {

        private readonly int OUTPUT_SIZE = 1;
        private readonly double INACTIVE_THRESHOLD = 0.25;
        private readonly double ACTIVE_THRESHOLD = 0.75;

        public bool JudgeNetwork(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            return (outputs[0].Activation < INACTIVE_THRESHOLD && Item.expectedResult == 0) || (outputs[0].Activation > ACTIVE_THRESHOLD && Item.expectedResult == 1);
        }

        public int[] TrainingResult(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            int[] shouldActivate = new int[outputs.Length];
            shouldActivate[0] = Item.expectedResult == 1 ? 1 : 0;
            return shouldActivate;
        }

        private void TryValidParamters(OutputNode[] outputs)
        {
            if (outputs.Length != OUTPUT_SIZE)
            {
                throw new Exception("Output size does not match this judge.");
            }
        }

        public Dictionary<int, AccuracyData> getEmptyAccuracyInfo()
        {
            Dictionary<int, AccuracyData> _Accuracy = new Dictionary<int, AccuracyData>();
            for (int i = 0; i < 2; i++)
            {
                _Accuracy[i] = new AccuracyData();
            }
            return _Accuracy;
        }


        public int JudgeNetwork(OutputNode[] outputs)
        {
            return outputs[0].Activation > ACTIVE_THRESHOLD ? 1 : 0;
        }
    }
}
