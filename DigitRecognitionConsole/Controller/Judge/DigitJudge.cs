using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionDisplay.Model;
using System.IO;

namespace DigitRecognitionDisplay.Controller
{
    public class DigitJudge : IJudge
    {
        private readonly int OUTPUT_SIZE = 10;

        public bool JudgeNetwork(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            int MostActiveResult = -1;
            double highestActivation = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i].Activation > highestActivation)
                {
                    MostActiveResult = i;
                    highestActivation = outputs[i].Activation;
                }
            }
            return MostActiveResult == Item.expectedResult;
        }

        public bool JudgeNetwork(DataItem Item, OutputNode[] outputs, StreamWriter writer)
        {
            TryValidParamters(outputs);
            int MostActiveResult = -1;
            double highestActivation = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i].Activation > highestActivation)
                {
                    MostActiveResult = i;
                    highestActivation = outputs[i].Activation;
                }
            }
            bool result = MostActiveResult == Item.expectedResult;

            if(!result)
            {
                writer.Write(MostActiveResult + ", ");
            }
            return result;
        }

        public int[] TrainingResult(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            int[] ShouldActivate = new int[outputs.Length];
            //Instantiates to all false, and I want one true where the number is correct.
            ShouldActivate[Item.expectedResult] = 1;
            return ShouldActivate;
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
            for (int i = 0; i < 10; i++)
            {
                _Accuracy[i] = new AccuracyData();
            }
            return _Accuracy;
        }
    }
}
