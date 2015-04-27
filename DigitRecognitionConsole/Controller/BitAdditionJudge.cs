﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class BitAdditionJudge : IJudge
    {
        private readonly int OUTPUT_SIZE = 3;
        private readonly double INACTIVE_THRESHOLD = 0.25;
        private readonly double ACTIVE_THRESHOLD = 0.75;

        public bool JudgeNetwork(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            int networkValue = NetworkValue(outputs);
            return networkValue == Item.expectedResult;
        }

        public bool[] TrainingResult(DataItem Item, OutputNode[] outputs)
        {
            TryValidParamters(outputs);
            int networkValue = NetworkValue(outputs);
            bool[] ShouldActivate = new bool[outputs.Length];
            for (int i = 0; i < ShouldActivate.Length; i++)
            {
                ShouldActivate[i] = (Item.expectedResult & (1 << outputs.Length - 1 - i)) != 0; 
            }
            return ShouldActivate;
        }

        private int NetworkValue(OutputNode[] outputs)
        {
            int networkValue = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                networkValue += ActiveVal(outputs[i]) * (1 << outputs.Length - 1 - i);
            }
            return networkValue;
        }

        private int ActiveVal(OutputNode node)
        {
            return node.Activation > ACTIVE_THRESHOLD ? 1 : 0;
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