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

namespace ImageInput
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap imageToGuess;
        private double[] data;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImageSelector_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Fdlg = new OpenFileDialog();
            Fdlg.Filter = "Images|*.jpeg;*.png;*.jpg;*.gif";
            Fdlg.Title = "Character to guess";
            Stream stream;

            if (Fdlg.ShowDialog() == true)
            {
                stream = Fdlg.OpenFile();
                imageToGuess = new Bitmap(stream);
                //imageToGuess = new Bitmap(Fdlg.OpenFile());
                imageToGuess = MakeGrayscale3(imageToGuess);
                imageToGuess = new Bitmap(imageToGuess, 28, 28);
                MemoryStream ms = new MemoryStream();
                imageToGuess.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                int imageSize = imageToGuess.Height * imageToGuess.Width;
                byte[] rawData = new byte[imageSize];
                ms.Read(rawData, 0, imageSize);
                ms.Position = 0;

                data = rawData.Cast<double>().ToArray();

                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                image.Source = bi;
                Canvas.SetTop(image, 0);
                Canvas.SetLeft(image, 0);
                ImageViewer.Children.Add(image);

                //stream.Close();
            }
        }

        private int TestImage()
        {
            PersistentNetwork StoredNetwork = null;
            string FileName = @"HalfHiddenRate1";
            string FilePath = @"..\..\..\DigitRecognitionConsole\Data\";

            IJudge judge = new DigitJudge();
            try
            {
                StoredNetwork = NetworkPersist.LoadNetwork(FileName, FilePath);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Bad Path");
            }

            StoredNetwork.Network.judgeInput(data);

            return 0;
        }

        private Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][] 
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    }
}
