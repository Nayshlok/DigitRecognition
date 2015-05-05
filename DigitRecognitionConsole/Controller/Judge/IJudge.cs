using DigitRecognitionDisplay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionDisplay.Controller
{
    public interface IJudge
    {
        bool JudgeNetwork(DataItem Item, OutputNode[] outputs);
        int[] TrainingResult(DataItem Item, OutputNode[] outputs);
        Dictionary<int, AccuracyData> getEmptyAccuracyInfo();
    }
}
