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

        private readonly int pixelSize = 1;
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
