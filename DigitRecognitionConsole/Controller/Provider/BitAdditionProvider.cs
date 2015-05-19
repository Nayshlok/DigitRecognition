using System;
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
        private readonly int BIT_HIDDENSIZE = 12;
        private readonly int BIT_INPUTS = 4;
        private static Random rand = new Random();
        private readonly int NUMBER_OF_SETS = 100000;

        public int GetPossibleOutputs()
        {
            return BIT_OUTPUTS;
        }

        public int GetNumOfInputs()
        {
            return BIT_INPUTS;
        }

        public IEnumerable<DataItem> GetDataItems()
        {
            for (int i = 0; i < NUMBER_OF_SETS; i++)
            {
                double[] data = new double[] { rand.Next(2), rand.Next(2), rand.Next(2), rand.Next(2) };
                int expected = (int)((data[0] * 2 + data[1]) + (data[2] * 2 + data[3]));
                yield return new DataItem { data = data, expectedResult = expected };
            }
        }

        public int GetSetSize()
        {
            return NUMBER_OF_SETS;
        }

        public int[] GetHiddenLayerSizes()
        {
            return new int[] {10*BIT_HIDDENSIZE};
        }


        public IEnumerable<DataItem> GetConditionalDataItems(Predicate<int> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
