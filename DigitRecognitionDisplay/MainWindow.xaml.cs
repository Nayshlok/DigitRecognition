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
using DigitRecognitionConsole.Controller;
using DigitRecognitionConsole.Model;
using System.Windows.Threading;

namespace DigitRecognitionDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int NUMBER_OF_EPOCHS = 1000;
        private readonly int EPOCH_SIZE = 300;
        private readonly int POINT_RADIUS = 4;

        private static double height;
        private static double EpochDistance;

        private DispatcherTimer timer;
        private Driver driver;
        private double[] data;
        private int currentIndex;
        private Ellipse prevPoint;

        public MainWindow()
        {
            InitializeComponent();

            DrawVerticalScale(1.0, 0.0, 10);

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000);
            timer.Tick += OnTimedEvent;
            IDataProvider provider = new BinaryXORProvider();
            //IDataProvider provider = new DigitProvider(Program.TestingDataPath, Program.TestingLabelPath);
            IJudge judge = new XORJudge();
            driver = new Driver(provider, judge);

            height = MSEGraph.Height;
            EpochDistance = MSEGraph.Width / NUMBER_OF_EPOCHS;

            data = driver.EpochTrain(EPOCH_SIZE, NUMBER_OF_EPOCHS).ToArray();

            timer.Start();

        }

        public void OnTimedEvent(object source, EventArgs e)
        {

            if(currentIndex < data.Length){
                Ellipse point = PlotPoint(data[currentIndex], currentIndex);
                if (prevPoint != null)
                {
                    ConnectPoints(prevPoint, point);
                }
                prevPoint = point;
                currentIndex++;
            }
            else
            {
                Label totalAccuracy = new Label();
                //IDataProvider testProvider = new DigitProvider(Program.TrainingDataPath, Program.TrainingLabelPath);
                IDataProvider testProvider = new BinaryXORProvider();

                var testData = driver.TestNetwork(testProvider, 200);
                totalAccuracy.Content = "Total Accuracy: " + driver.TotalAccuracy;
                AccuracyInfo.Children.Add(totalAccuracy);
                foreach (KeyValuePair<int, AccuracyData> kv in testData)
                {
                    Label accuracy = new Label();
                    accuracy.Content = kv.Key + ": " + kv.Value.ToString();
                    AccuracyInfo.Children.Add(accuracy);
                }
                timer.Stop();
            }
        }

        public Ellipse PlotPoint(double MSE, int Epoch)
        {
            Ellipse point = new Ellipse();
            point.Width = POINT_RADIUS;
            point.Height = POINT_RADIUS;
            point.Fill = new SolidColorBrush(Colors.DarkBlue);
            double topValue = (1 - MSE) * height;
            Canvas.SetTop(point, (1 - MSE) * height);
            Canvas.SetLeft(point, (Epoch * EpochDistance) + 25);
            MSEGraph.Children.Add(point);
            return point;
        }

        public void DrawVerticalScale(double top, double bot, int steps)
        {
            double stepSize = (top - bot) / steps;
            double current = top;
            for (int i = 0; i <= steps; i++)
            {
                Label l = new Label();
                l.Content = Math.Round(current, 2);
                Canvas.SetTop(l, (MSEGraph.Height / (steps + 1)) * i);
                Canvas.SetLeft(l, 0);
                MSEGraph.Children.Add(l);
                current -= stepSize;
            }
        }

        public void ConnectPoints(Ellipse p1, Ellipse p2)
        {
            Line line = new Line();
            line.X1 = Canvas.GetLeft(p1) + (POINT_RADIUS / 2);
            line.Y1 = Canvas.GetTop(p1) + (POINT_RADIUS / 2);
            line.X2 = Canvas.GetLeft(p2) + (POINT_RADIUS / 2);
            line.Y2 = Canvas.GetTop(p2) + (POINT_RADIUS / 2);
            line.Stroke = new SolidColorBrush(Colors.Blue);
            line.StrokeThickness = 1.5;
            MSEGraph.Children.Add(line);
        }
    }
}
