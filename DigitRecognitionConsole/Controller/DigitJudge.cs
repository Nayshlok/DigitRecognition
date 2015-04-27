using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
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
    }
}
