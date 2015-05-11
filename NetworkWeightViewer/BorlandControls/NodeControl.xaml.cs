using System;
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
using System.ComponentModel;

namespace NetworkWeightViewer.BorlandControls
{
    /// <summary>
    /// Interaction logic for NodeControl.xaml
    /// </summary>
    public partial class NodeControl : UserControl, INotifyPropertyChanged
    {
        private string _ObjectType;
        private string _ObjectName;
        private double _Error;
        private double _Activation;
        public double Error
        {
            get
            {
                return _Error;
            }
            set
            {
                _Error = value;
                FirePropertyChanged("Error");
            }
        }
        public double Activation
        {
            get
            {
                return _Activation;
            }
            set
            {
                _Activation = value;
                FirePropertyChanged("Activation");
            }
        }

        public NodeControl(string type, string name)
        {
            _ObjectType = type;
            _ObjectName = name;
            InitializeComponent();
            ObjectType.Content = _ObjectType;
            ObjectName.Content = _ObjectName;
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
