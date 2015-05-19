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
        private Bitmap imageToGuess;
        private double[] data;
        private PersistentNetwork StoredNetwork;

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
                int imageSize = imageToGuess.Height * imageToGuess.Width;

                data = NormalizeByteData(rawData);
                int test = TestImage();
                NumberGuess.Content = test;
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



        //External Code. It works well for gray scaling an image.
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

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(System.Drawing.Image image)
        {
            int width = 28;
            int height = 28;
            var destRect = new System.Drawing.Rectangle(0, 0, width - 8, height - 8);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
