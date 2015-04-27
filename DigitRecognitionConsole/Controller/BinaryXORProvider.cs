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
        private readonly int XOR_HIDDEN_SIZE = 2;
        private readonly int NUMBER_OF_SETS = 1000000;

        public int[] GetPossibleOutputs()
        {
            return new int[]{ 0};
        }

        public int GetNumOfInputs()
        {
            return XOR_INPUTS;
        }

        public IEnumerable<DataItem> GetNextDataItem()
        {
            for (int i = 0; i < NUMBER_OF_SETS; i++)
            {
                byte[] data = new byte[] { (byte)rand.Next(2), (byte)rand.Next(2) };
                //byte[] data = new byte[] { 1, 0 };
                int expected = data[0] ^ data[1];
                yield return new DataItem { data = data, expectedResult = expected };
            }
            //return new DataItem { data = data, expectedResult = expected };
        }

        public int GetSetSize()
        {
            return NUMBER_OF_SETS;
        }


        public int GetHiddenLayerSize()
        {
            return XOR_HIDDEN_SIZE;
        }
    }
}
