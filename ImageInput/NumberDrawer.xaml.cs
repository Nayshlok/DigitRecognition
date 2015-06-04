using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace ImageInput
{
    /// <summary>
    /// Interaction logic for NumberDrawer.xaml
    /// </summary>
    public partial class NumberDrawer : UserControl
    {
        public NumberDrawer()
        {
            InitializeComponent();
        }

        private readonly int pixelSize = 2;
        private Dictionary<Point, ColoredBox> canvasPoints = new Dictionary<Point,ColoredBox>();

        public void drawAtPoint(Point p)
        {
            colorAtPoint(p, true);
            if (p.Y > 0)
            {
                colorAtPoint(new Point(p.X, p.Y - 1), false);
            }
            if (p.Y < DigitCanvas.Height - 1)
            {
                colorAtPoint(new Point(p.X, p.Y + 1), false);
            }
            if (p.X > 0)
            {
                colorAtPoint(new Point(p.X - 1, p.Y), false);
            }
            if (p.X < DigitCanvas.Width - 1)
            {
                colorAtPoint(new Point(p.X + 1, p.Y), false);
            }
        }

        public System.Drawing.Bitmap getBitmapFromDrawing()
        {
            int width = (int)DigitCanvas.Width/pixelSize;
            int height = (int)DigitCanvas.Height / pixelSize;
            var b = new System.Drawing.Bitmap((int)DigitCanvas.Width/pixelSize, (int)DigitCanvas.Height/pixelSize, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            ColorPalette ncp = b.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
            b.Palette = ncp;

            var BoundsRect = new System.Drawing.Rectangle(0, 0, (int)DigitCanvas.Width/pixelSize, (int)DigitCanvas.Height/pixelSize);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride*b.Height;
            var rgbValues = new byte[bytes];

            for (int i = 0; i < rgbValues.Length; i++)
            {
                int x = i % (b.Width/pixelSize);
                int y = i / (b.Width/pixelSize);
                ColoredBox selected;
                rgbValues[i] = canvasPoints.TryGetValue(new Point(x, y), out selected) ? selected.color.B : (byte)255;
            }

            Marshal.Copy(rgbValues, 0, ptr, bytes);
            b.UnlockBits(bmpData);
            return b;
        } 

        public void colorAtPoint(Point p, bool primary)
        {
            ColoredBox box;
            if (canvasPoints.TryGetValue(p, out box))
            {
                box.color = primary ? Colors.Black : shiftColorDown(box.color, (byte)64);
                box.rect.Fill = new SolidColorBrush(box.color);
            }
            else
            {
                box = new ColoredBox();
                box.rect = new Rectangle { Width = 1, Height = 1 };
                box.color = primary ? Colors.Black : Colors.Gray;
                box.rect.Fill = new SolidColorBrush(box.color);
                Canvas.SetTop(box.rect, p.Y);
                Canvas.SetLeft(box.rect, p.X);
                DigitCanvas.Children.Add(box.rect);
            }
        }

        private Color shiftColorDown(Color original, byte shiftAmount)
        {
            byte B = (byte)((original.B - shiftAmount < 0) ? original.B - shiftAmount : 0);

            return Color.FromArgb(255, B, B, B);
        }

        private bool isMouseDown = false;

        private void DigitCanvas_MousePress(object sender, MouseButtonEventArgs e)
        {
            Point press = e.GetPosition(DigitCanvas);
            drawAtPoint(press);
            isMouseDown = !isMouseDown;
        }

        private void DigitCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point press = e.GetPosition(DigitCanvas);
                drawAtPoint(press);
            }
        }

        private class ColoredBox
        {
            public Rectangle rect;
            public Color color;
        } 

    }
}
