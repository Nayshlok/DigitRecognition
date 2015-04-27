using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class XORJudge : IJudge
    {

        private readonly int INPUT_SIZE = 2;
        private readonly int OUTPUT_SIZE = 1;

        public bool JudgeNetwork(byte[] data, OutputNode[] outputs)
        {
            TryValidParamters(data, outputs);
            int expectedResult = data[0] ^ data[1];
            bool result = false;
            if (expectedResult == 0)
            {
                result = outputs[0].Activation > outputs[1].Activation;
            }
            else
            {
                result = outputs[0].Activation < outputs[1].Activation;
            }
            return result;
        }

        public bool[] TrainingResult(byte[] data, OutputNode[] outputs)
        {
            TryValidParamters(data, outputs);
        }

        private void TryValidParamters(byte[] data, OutputNode[] outputs)
        {
            if (data.Length != INPUT_SIZE)
            {
                throw new Exception("Input recieved does not match this judge");
            }
            if (outputs.Length != OUTPUT_SIZE)
            {
                throw new Exception("Output size does not match this judge.");
            }
        }
    }
}
