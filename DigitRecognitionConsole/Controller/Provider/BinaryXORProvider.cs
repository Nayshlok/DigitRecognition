using DigitRecognitionConsole.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Controller
{
    public class BinaryXORProvider : IDataProvider
    {
        private static Random rand = new Random();
        private readonly int XOR_INPUTS = 2;
        private readonly int XOR_HIDDEN_SIZE = 3;
        private readonly int XOR_OUTPUTS = 1;
        private readonly int NUMBER_OF_SETS = 100000;

        public int GetPossibleOutputs()
        {
            return XOR_OUTPUTS;
        }

        public int GetNumOfInputs()
        {
            return XOR_INPUTS;
        }

        public IEnumerable<DataItem> GetDataItems()
        {
            for (int i = 0; i < NUMBER_OF_SETS; i++)
            {
                double[] data = new double[] { rand.Next(2), rand.Next(2) };

                int expected = (int)data[0] ^ (int)data[1];
                yield return new DataItem { data = data, expectedResult = expected };
            }
        }

        public int GetSetSize()
        {
            return NUMBER_OF_SETS;
        }

        public int[] GetHiddenLayerSizes()
        {
            return new int[] { XOR_HIDDEN_SIZE };
        }


        public IEnumerable<DataItem> GetConditionalDataItems(Predicate<int> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
