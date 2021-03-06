﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkWeightViewer.BorlandControls
{
    /// <summary>
    /// Interaction logic for InputControl.xaml
    /// </summary>
    public partial class InputControl : UserControl
    {
        private string NameValue;

        public InputControl(string name)
        {
            this.NameValue = name;
            InitializeComponent();
        }

        public double GetInputValue()
        {
            return double.Parse(InputValue.Text);
        }
    }
}
