using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
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

        public System.Drawing.Bitmap getBitmapFromDrawing(int width, int height)
        {
            RenderTargetBitmap targetBM = new RenderTargetBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Default);
            targetBM.Render(DigitCanvas);
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(targetBM));
            System.Drawing.Bitmap bm;
            MemoryStream ms = new MemoryStream();
            encoder.Save(ms);
            ms.Position = 0;
            bm = new System.Drawing.Bitmap(ms);
            return bm;
            
        } 

        public void ClearCanvas()
        {
            DigitCanvas.Strokes.Clear();
        }

    }
}
