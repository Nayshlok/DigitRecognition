using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole
{
    class DataImageProcessing
    {
        private readonly int WIDTH = 28;
        private readonly int HEIGHT = 28;
        private readonly int BPP = 1; //Bits per pixel

        public Bitmap ProcessImage(byte[] ImageData)
        {
            Rectangle BoundImage = BoundingRectangle(ImageData);
            Bitmap croppedImage = original.Clone(BoundImage, original.PixelFormat);
            Bitmap smallImage = ResizeImage(croppedImage, (croppedImage.Width > 20 ? 20 : croppedImage.Width), croppedImage.Height > 20 ? 20 : croppedImage.Height);
            Bitmap finalImage = new Bitmap(28, 28);
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, finalImage.Width, finalImage.Height);
                Rectangle destRect = new Rectangle((finalImage.Width / 2) - (smallImage.Width/2), (finalImage.Height / 2) - center.Y, smallImage.Width, smallImage.Height);
                g.DrawImage(smallImage, destRect, 0, 0, smallImage.Width, smallImage.Height, GraphicsUnit.Pixel);
            }

            return finalImage;
        }

        private byte[] GetImageData(Bitmap image)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

            BitmapData bmData = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);
            IntPtr ptr = bmData.Scan0;
            int bytes = Math.Abs(bmData.Stride) * bmData.Height;
            byte[] imageData = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, imageData, 0, bytes);
            image.UnlockBits(bmData);

            return imageData;
        }

        private Rectangle BoundingRectangle(byte[] imageData)
        {
            int rectX = WIDTH;
            int rectY = HEIGHT;
            int rectWidth = 0;
            int rectHeight = 0;
            int farRight = 0;
            int bottom = 0;
            bool firstPixel = true;
            for (int i = 1; i < imageData.Length; i += BPP)
            {
                if (imageData[i] < 250)
                {
                    int x = (i) % WIDTH;
                    int y = (i) / WIDTH;
                    if (x > farRight)
                    {
                        farRight = x;
                    }
                    if (y > bottom)
                    {
                        bottom = y;
                    }
                    if (x < rectX)
                    {
                        rectX = x;
                    }
                    if (y < rectY)
                    {
                        rectY = y;
                    }
                    int widthAtPoint = Math.Abs(farRight - rectX) + 1;
                    int heightAtPoint = Math.Abs(bottom - rectY) + 1;

                    if (firstPixel)
                    {
                        widthAtPoint = 1;
                        heightAtPoint = 1;
                        firstPixel = false;
                    }

                    if (widthAtPoint > rectWidth)
                    {
                        rectWidth = widthAtPoint;
                    }
                    if (heightAtPoint > rectHeight)
                    {
                        rectHeight = heightAtPoint;
                    }

                }
            }
            return new Rectangle(rectX, rectY, rectWidth, rectHeight);
        }

        private byte[] CropImage()
        {

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
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
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

        private class CroppedImage
        {
            byte[] data;
            int width;
            int height;
        }
    }
}
