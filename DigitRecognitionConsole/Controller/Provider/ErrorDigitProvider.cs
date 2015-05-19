using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Controller.Provider
{
    public class ErrorDigitProvider : IDataProvider
    {

        public int GetPossibleOutputs()
        {
            throw new NotImplementedException();
        }

        public int GetNumOfInputs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.DataItem> GetDataItems()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.DataItem> GetConditionalDataItems(Predicate<int> predicate)
        {
            throw new NotImplementedException();
        }

        public int GetSetSize()
        {
            throw new NotImplementedException();
        }

        public int[] GetHiddenLayerSizes()
        {
            throw new NotImplementedException();
        }
    }
}
