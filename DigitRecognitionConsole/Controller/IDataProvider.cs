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
        int[] GetPossibleOutputs();
        int GetNumOfInputs();
        DataItem GetNextDataItem();
        int GetTrainingSetSize();
    }
}
