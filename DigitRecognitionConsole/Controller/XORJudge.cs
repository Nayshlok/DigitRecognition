﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class XORJudge : IJudge
    {

        private readonly int OUTPUT_SIZE = 1;
        private readonly double INACTIVE_THRESHOLD = 0.25;
        private readonly double ACTIVE_THRESHOLD = 0.75;

        public bool JudgeNetwork(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            bool result = (outputs[0].Activation < INACTIVE_THRESHOLD && Item.expectedResult == 0) || (outputs[0].Activation > ACTIVE_THRESHOLD && Item.expectedResult == 1);
            return result;
        }

        public bool[] TrainingResult(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            bool[] shouldActivate = new bool[outputs.Length];
            shouldActivate[0] = Item.expectedResult == 1;
            return shouldActivate;
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
