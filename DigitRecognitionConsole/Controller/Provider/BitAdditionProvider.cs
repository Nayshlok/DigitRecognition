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
        public static readonly int BIT_OUTPUTS = 5;
        private readonly int BIT_HIDDENSIZE = 15;
        private readonly int BIT_INPUTS = 8;
        private readonly int NUMBER_SIZE = 4;
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
                double[] data = new double[BIT_INPUTS];
                int expected = 0;
                for (int j = 0; j < data.Length; j++)
                {
                    int bit = rand.Next(2);
                    data[j] = bit;
                    expected +=(int)( bit * Math.Pow(2, NUMBER_SIZE - 1 - (j % NUMBER_SIZE)));
                }
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
