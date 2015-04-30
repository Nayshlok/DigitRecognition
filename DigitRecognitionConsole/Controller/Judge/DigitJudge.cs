using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;
using System.IO;

namespace DigitRecognitionConsole.Controller
{
    public class DigitJudge : IJudge
    {
        private readonly int OUTPUT_SIZE = 10;
        private Dictionary<int, AccuracyData> _Accuracy;

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
            bool result = MostActiveResult == Item.expectedResult;

            _Accuracy[Item.expectedResult].total++;
            if (result)
            {
                _Accuracy[Item.expectedResult].correct++;
            }

            return result;
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

            _Accuracy[Item.expectedResult].total++;
            if (result)
            {
                _Accuracy[Item.expectedResult].correct++;
            }
            else
            {
                writer.Write(MostActiveResult + ", ");
            }
            return result;
        }

        public bool[] TrainingResult(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            bool[] ShouldActivate = new bool[outputs.Length];
            //Instantiates to all false, and I want one true where it should be.
            ShouldActivate[Item.expectedResult] = true;
            return ShouldActivate;
        }

        private void TryValidParamters(OutputNode[] outputs)
        {
            if (outputs.Length != OUTPUT_SIZE)
            {
                throw new Exception("Output size does not match this judge.");
            }
        }


        public void ResetTraining()
        {
            _Accuracy = new Dictionary<int, AccuracyData>();
            for (int i = 0; i < 10; i++)
            {
                _Accuracy[i] = new AccuracyData();
            }
        }

        public Dictionary<int, AccuracyData> getAccuracy()
        {
            return _Accuracy;
        }
    }
}
