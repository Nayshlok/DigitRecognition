﻿using DigitRecognitionConsole.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Controller
{
    public interface IJudge
    {
        bool JudgeNetwork(DataItem Item, OutputNode[] outputs);
        bool[] TrainingResult(DataItem Item, OutputNode[] outputs);

    }
}