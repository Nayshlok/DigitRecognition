using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl, INotifyPropertyChanged
    {
        private string _SenderName;
        private string _ReceiverName;
        private double _Weight;
        public double Weight
        {
            get
            {
                return Math.Round(_Weight, 4);
            }
            set
            {
                _Weight = value;
                FirePropertyChanged("Weight");
            }
        }

        public ConnectionControl(string sender, string receiver)
        {
            this._SenderName = sender;
            this._ReceiverName = receiver;
            DataContext = this;
            InitializeComponent();
            SenderName.Content = sender;
            ReceiverName.Content = receiver;
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
