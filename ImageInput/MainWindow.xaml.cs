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
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;
using DigitRecognitionConsole.Model;
using DigitRecognitionConsole.Controller;
using System.Drawing.Drawing2D;

namespace ImageInput
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int PIXELSIZE = 1;
        private Bitmap imageToGuess;
        private double[] data;
        private PersistentNetwork StoredNetwork;
        private ImageProcessing imageProcessor = new ImageProcessing();
        NumberDrawer drawer;


        public MainWindow()
        {
            string FileName = @"HalfHiddenRate1Record";
            string FilePath = @"..\..\..\DigitRecognitionConsole\Data\";
            try
            {
                StoredNetwork = NetworkPersist.LoadNetwork(FileName, FilePath);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Bad Path");
            }
            InitializeComponent();
            drawer = new NumberDrawer();
            Grid.SetRow(drawer, 2);
            Grid.SetColumn(drawer, 2);
            MainGrid.Children.Add(drawer);
        }

        private void ImageSelector_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Fdlg = new OpenFileDialog();
            Fdlg.Filter = "Images|*.jpeg;*.png;*.jpg;*.gif";
            Fdlg.Title = "Character to guess";

            if (Fdlg.ShowDialog() == true)
            {
                Bitmap originalImage = new Bitmap(Fdlg.OpenFile());
                DisplayAndTestImage(originalImage);
            }
        }

        private void CanvasSelector_Click(object sender, RoutedEventArgs e)
        {
            int width = (int)MainGrid.ActualWidth / MainGrid.ColumnDefinitions.Count;
            int height = (int)MainGrid.ActualHeight / MainGrid.RowDefinitions.Count;
            Bitmap original = drawer.getBitmapFromDrawing(width, height);
            DisplayAndTestImage(original);
        }

        private void DisplayAndTestImage(Bitmap originalImage)
        {
            imageToGuess = imageProcessor.ProcessImage(originalImage);

            byte[] rawData = new byte[imageToGuess.Width * imageToGuess.Height];
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, imageToGuess.Width, imageToGuess.Height);

            BitmapData bmData = imageToGuess.LockBits(rect, ImageLockMode.ReadWrite, imageToGuess.PixelFormat);
            IntPtr ptr = bmData.Scan0;
            int bytes = Math.Abs(bmData.Stride) * bmData.Height;
            byte[] imageData = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, imageData, 0, bytes);


            for (int i = 1, j = 0; i < imageData.Length; i += 4, j++)
            {
                rawData[j] = (byte)(255 - imageData[i]); //(byte)(imageData[i] < 240 ? 255 : 0);
            }

            imageToGuess.UnlockBits(bmData);

            MemoryStream ms = new MemoryStream();
            originalImage.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            DrawImage(rawData, imageToGuess.Height, imageToGuess.Width);
            data = NormalizeByteData(rawData);
            int test = TestImage();
            NumberGuess.Content = test;
            ChanceView.Children.Clear();
            for (int i = 0; i < StoredNetwork.Network.outputNodes.Length; i++)
            {
                Label guess = new Label();
                guess.Content = i + ": " + Math.Round((StoredNetwork.Network.outputNodes[i].Activation * 100), 4);
                ChanceView.Children.Add(guess);
            }

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            image.Source = bi;
            Canvas.SetTop(image, 0);
            Canvas.SetLeft(image, 0);
            ImageViewer.Children.Clear();
            ImageViewer.Children.Add(image);
        }

        private int TestImage()
        {
            IJudge judge = new DigitJudge();
            return judge.JudgeNetwork(StoredNetwork.Network.judgeInput(data));
        }

        private double[] NormalizeByteData(byte[] rawData){
            double[] normalizedData = new double[rawData.Length];
            for (int i = 0; i < normalizedData.Length; i++)
            {
                normalizedData[i] = (double)rawData[i] / 255d;
            }
            return normalizedData;
        }

        public void DrawImage(byte[] data, int height, int width)
        {
            DrawnImage.Children.Clear();
            for (int i = 0; i < height + 2; i++)
            {
                for (int j = 0; j < width + 2; j++)
                {
                    if (i == 0 || i == height + 1)
                    {
                        DrawBox(DrawnImage, i, j, 255);
                    }
                    else if (j == 0 || j == width + 1)
                    {
                        DrawBox(DrawnImage, i, j, 255);
                    }
                    else
                    {
                        DrawBox(DrawnImage, i, j, data[((i - 1) * width) + j - 1]);
                    }
                }
            }
        }

        public void DrawBox(Canvas ImageCanvas, int y, int x, byte value)
        {
            System.Windows.Shapes.Rectangle singlePixel = new System.Windows.Shapes.Rectangle();
            singlePixel.Width = PIXELSIZE;
            singlePixel.Height = PIXELSIZE;
            singlePixel.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 - value), (byte)(255 - value), (byte)(255 - value)));
            Canvas.SetTop(singlePixel, y * PIXELSIZE);
            Canvas.SetLeft(singlePixel, x * PIXELSIZE);
            ImageCanvas.Children.Add(singlePixel);
        }

        private void OffsetAndTest_Click(object sender, RoutedEventArgs e)
        {
            int x = (int)Xoffset.Value;
            int y = (int)YOffset.Value;

            Bitmap imageToGuess = imageProcessor.offsetImage(x, y);

            byte[] rawData = new byte[imageToGuess.Width * imageToGuess.Height];
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, imageToGuess.Width, imageToGuess.Height);

            BitmapData bmData = imageToGuess.LockBits(rect, ImageLockMode.ReadWrite, imageToGuess.PixelFormat);
            IntPtr ptr = bmData.Scan0;
            int bytes = Math.Abs(bmData.Stride) * bmData.Height;
            byte[] imageData = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, imageData, 0, bytes);


            for (int i = 1, j = 0; i < imageData.Length; i += 4, j++)
            {
                rawData[j] = (byte)(255 - imageData[i]); //(byte)(imageData[i] < 240 ? 255 : 0);
            }

            imageToGuess.UnlockBits(bmData);

            MemoryStream ms = new MemoryStream();
            imageToGuess.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            DrawImage(rawData, imageToGuess.Height, imageToGuess.Width);
            data = NormalizeByteData(rawData);
            int test = TestImage();
            NumberGuess.Content = test;
            ChanceView.Children.Clear();
            for (int i = 0; i < StoredNetwork.Network.outputNodes.Length; i++)
            {
                Label guess = new Label();
                guess.Content = i + ": " + Math.Round((StoredNetwork.Network.outputNodes[i].Activation * 100), 4);
                ChanceView.Children.Add(guess);
            }

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            image.Source = bi;
            Canvas.SetTop(image, 0);
            Canvas.SetLeft(image, 0);
            ImageViewer.Children.Clear();
            ImageViewer.Children.Add(image);
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            drawer.ClearCanvas();
        }

    }
}
