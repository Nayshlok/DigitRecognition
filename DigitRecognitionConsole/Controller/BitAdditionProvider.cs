﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class BitAdditionProvider : IDataProvider
    {
        private readonly int BIT_OUTPUTS = 3;
        private readonly int BIT_HIDDENSIZE = 3;
        private readonly int BIT_INPUTS = 4;
        private static Random rand = new Random();
        private readonly int NUMBER_OF_SETS = 1000000;

        public int GetPossibleOutputs()
        {
            return BIT_OUTPUTS;
        }

        public int GetNumOfInputs()
        {
            return BIT_INPUTS;
        }

        public IEnumerable<DataItem> GetNextDataItem()
        {
            for (int i = 0; i < NUMBER_OF_SETS; i++)
            {
                byte[] data = new byte[] { (byte)rand.Next(2), (byte)rand.Next(2), (byte)rand.Next(2), (byte)rand.Next(2) };
                int expected = (data[0] * 2 + data[1]) + (data[2] * 2 + data[3]);
                yield return new DataItem { data = data, expectedResult = expected };
            }
        }

        public int GetSetSize()
        {
            return NUMBER_OF_SETS;
        }

        public int GetHiddenLayerSize()
        {
            return BIT_HIDDENSIZE;
        }
    }
}