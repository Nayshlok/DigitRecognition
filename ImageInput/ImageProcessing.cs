using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInput
{
    class ImageProcessing
    {
        private Bitmap StoredSmallImage;

        public Bitmap ProcessImage(Bitmap original)
        {
            Bitmap GrayScaledImage = MakeGrayscale3(original);
            Rectangle BoundImage = BoundingRectangle(GrayScaledImage);
            Bitmap croppedImage = original.Clone(BoundImage, original.PixelFormat);
            Bitmap smallImage = ResizeImage(croppedImage, (croppedImage.Width > 20 ? 20 : croppedImage.Width), croppedImage.Height > 20 ? 20 : croppedImage.Height);
            StoredSmallImage = smallImage;
            //Bitmap smallImage = new Bitmap(croppedImage, 20, 20);
            Bitmap finalImage = new Bitmap(28, 28);
            Point center = CenterOfMass(smallImage);
            if (center.X < 6)
                center.X = 6;
            if (center.X > 22)
                center.X = 22;
            if (center.Y < 6)
                center.Y = 6;
            if (center.Y > 22)
                center.Y = 22;
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, finalImage.Width, finalImage.Height);
                Rectangle destRect = new Rectangle((finalImage.Width / 2) - (smallImage.Width/2), (finalImage.Height / 2) - center.Y, smallImage.Width, smallImage.Height);
                g.DrawImage(smallImage, destRect, 0, 0, smallImage.Width, smallImage.Height, GraphicsUnit.Pixel);
            }

            return finalImage;
        }

        public Bitmap offsetImage(int x, int y)
        {
            if (StoredSmallImage == null)
            {
                throw new Exception();
            }
            Bitmap transposedImage = new Bitmap(28, 28);
            using (Graphics g = Graphics.FromImage(transposedImage))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, transposedImage.Width, transposedImage.Height);
                Rectangle destRect = new Rectangle((transposedImage.Width / 2) - x, (transposedImage.Height / 2) - y, StoredSmallImage.Width, StoredSmallImage.Height);
                g.DrawImage(StoredSmallImage, destRect, 0, 0, StoredSmallImage.Width, StoredSmallImage.Height, GraphicsUnit.Pixel);
            }
            return transposedImage;
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

        private Point CenterOfMass(Bitmap image)
        {
            byte[] imageData = GetImageData(image);
            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int xWeight = 0;
            int yWeight = 0;
            int totalWeight = 0;
            int counter = 0;
            for (int i = 1; i < imageData.Length; i+= bytesPerPixel)
            {
                xWeight += imageData[i] * (((i/bytesPerPixel) % image.Width) + 1);
                yWeight += imageData[i] * (((i/bytesPerPixel) / image.Width) + 1);
                totalWeight += imageData[i];
                counter++;
            }
            return new Point(xWeight / totalWeight, yWeight/totalWeight);
        }

        private Rectangle BoundingRectangle(Bitmap image)
        {
            byte[] imageData = GetImageData(image);
            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            return BoundingRectangle(imageData, image.Height, image.Width, bytesPerPixel);
        }

        private Rectangle BoundingRectangle(byte[] imageData, int height, int width, int bytesPerPixel)
        {
            int rectX = width;
            int rectY = height;
            int rectWidth = 0;
            int rectHeight = 0;
            int farRight = 0;
            int bottom = 0;
            bool firstPixel = true;
            for (int i = 1; i < imageData.Length; i += bytesPerPixel)
            {
                if (imageData[i] < 250)
                {
                    int x = (i / bytesPerPixel) % width;
                    int y = (i / bytesPerPixel) / width;
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

    }
}
