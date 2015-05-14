using DigitRecognitionConsole.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Controller
{
    public interface IDataProvider
    {
        int GetPossibleOutputs();
        int GetNumOfInputs();
        IEnumerable<DataItem> GetDataItems();
        IEnumerable<DataItem> GetConditionalDataItems(Predicate<int> predicate);
        int GetSetSize();
        int[] GetHiddenLayerSizes();
    }
}
