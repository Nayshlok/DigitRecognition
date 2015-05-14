using DigitRecognitionConsole.Controller;
using DigitRecognitionConsole.Model;
using NetworkWeightViewer.BorlandControls;
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


namespace NetworkWeightViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NeuralNet net;
        private IDataProvider provider;
        private IJudge judge;
        private InputControl[] inputs;
        private NodeControl[] nodes;
        private List<ConnectionControl> connections;

        public MainWindow()
        {
            connections = new List<ConnectionControl>();

            provider = new BinaryXORProvider();
            judge = new XORJudge();
            net = new NeuralNet(judge, provider.GetNumOfInputs(), provider.GetHiddenLayerSizes(), provider.GetPossibleOutputs());

            //foreach (DataItem nextItem in provider.GetNextDataItem().Take(provider.GetSetSize()))
            //{
            //    net.TrainNetwork(nextItem);
            //}

            InitializeComponent();

            inputs = new InputControl[net.inputNodes.Length];
            for (int i = 0; i < net.inputNodes.Length; i++)
            {
                inputs[i] = new InputControl(net.inputNodes[i].Name);
                InputOptions.Children.Add(inputs[i]);
            }
            CreateNetView();
        }

        public void CreateNetView()
        {
            Stack<UIElement> elements = new Stack<UIElement>();

            StackPanel outputLayer = new StackPanel();
            outputLayer.Orientation = Orientation.Vertical;

            StackPanel OutputConnection = new StackPanel();
            OutputConnection.Orientation = Orientation.Vertical;
            foreach (OutputNode n in net.outputNodes)
            {
                NodeControl node = new NodeControl(n.GetType().Name, n.Name);
                node.Activation = n.Activation;
                outputLayer.Children.Add(node);
                outputLayer.Children.Add(new Separator());
                foreach (NetConnection nc in n.Inputs)
                {
                    ConnectionControl connection = new ConnectionControl(nc.Sender.Name, nc.Receiver.Name);
                    connection.Weight = nc.Weight;
                    connections.Add(connection);
                    OutputConnection.Children.Add(connection);
                }
            }
            elements.Push(outputLayer);
            elements.Push(OutputConnection);

            ActivatingNode previous = net.outputNodes[0];
            while (previous != null)
            {
                StackPanel ConnectionLayer = new StackPanel();
                ConnectionLayer.Orientation = Orientation.Vertical;
                foreach (NetConnection n in previous.Inputs)
                {
                    
                    if (n.Sender is ActivatingNode)
                    {
                        ActivatingNode an = n.Sender as ActivatingNode;

                        foreach (NetConnection nc in an.Inputs)
                        {
                            ConnectionControl connection = new ConnectionControl(nc.Sender.Name, nc.Receiver.Name);
                            connection.Weight = nc.Weight;
                            connections.Add(connection);
                            ConnectionLayer.Children.Add(connection);
                        }
                        ConnectionLayer.Children.Add(new Separator());
                    }
                }
                elements.Push(ConnectionLayer);
                previous = previous.Inputs[0].Sender as ActivatingNode;
            }

            while (elements.Count != 0)
            {
                NetworkDisplay.Children.Add(elements.Pop());

            }
        }



        public void UpdateNetView()
        {
            NetworkDisplay.Children.Clear();
            connections.Clear();
            CreateNetView();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double[] inputValues = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputValues[i] = inputs[i].GetInputValue();
            }
            int expectedAnswer = int.Parse(ExpectedValue.Text);
            net.TrainNetwork(new DataItem { data = inputValues, expectedResult = expectedAnswer });
            UpdateNetView();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double[] inputValues = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputValues[i] = inputs[i].GetInputValue();
            }
            net.judgeInput(inputValues);
            UpdateNetView();
        }


    }
}
