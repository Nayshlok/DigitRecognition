using DigitRecognitionDisplay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionDisplay.Controller
{
    public interface IDataProvider
    {
        int GetPossibleOutputs();
        int GetNumOfInputs();
        IEnumerable<DataItem> GetNextDataItem();
        int GetSetSize();
        int[] GetHiddenLayerSizes();
    }
}
