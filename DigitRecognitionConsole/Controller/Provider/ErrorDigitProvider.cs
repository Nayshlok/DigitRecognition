using DigitRecognitionConsole.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognitionConsole.Controller
{
    public class ErrorDigitProvider : IDataProvider
    {

private readonly int NUM_OF_DIGITS = 10;
        //Metadata consists of 4 ints, a magic number, the number of images in the file, the number of columns, and the number of rows. 4 ints take 16 bytes, thus the offset.
        private readonly int METADATA_SIZE = 16;
        private readonly double MAX_PIXEL_INTENSITY = 255;
        private string DataPath;
        private string LabelPath;
        private int NumberOfImages;
        private int[] MissedIndexes;
        private int ImageSize;

        public ErrorDigitProvider(string DataPath, string LabelPath, string MissedIndexes)
        {
            this.DataPath = DataPath;
            this.LabelPath = LabelPath;
            ReadMetadata(MissedIndexes);
        }

        private void ReadMetadata(string MissedIndexFile)
        {
            using (FileStream stream = new FileStream(DataPath, FileMode.Open))
            {
                //Skip magic number and number of images
                stream.Seek(8, SeekOrigin.Begin);
                int row = ReadInt(stream);
                int col = ReadInt(stream);
                this.ImageSize = row * col;
            }
            string info;
            using (StreamReader reader = new StreamReader(MissedIndexFile))
            {
                info = reader.ReadToEnd();
            }
            string[] numberStrings = info.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            MissedIndexes = new int[numberStrings.Length];
            for (int i = 0; i < MissedIndexes.Length; i++)
            {
                int.TryParse(numberStrings[i], out MissedIndexes[i]);
            }
            NumberOfImages = MissedIndexes.Length;
        }

        public int GetPossibleOutputs()
        {
            return NUM_OF_DIGITS;
        }

        public int GetNumOfInputs()
        {
            return ImageSize;
        }

        public IEnumerable<DataItem> GetDataItems()
        {
            DataItem image = null;

            for (int i = 0; i < NumberOfImages; i++)
            {
                int label = FindLabel(MissedIndexes[i]);
                using (FileStream stream = new FileStream(DataPath, FileMode.Open))
                {
                    image = new DataItem { data = ReadSingleImage(stream, MissedIndexes[i]), expectedResult = label };
                }
                yield return image;
            }
        }


        public IEnumerable<DataItem> GetConditionalDataItems(Predicate<int> predicate)
        {
            DataItem image = null;

            for (int i = 0; i < NumberOfImages; i++)
            {
                int label = FindLabel(i);
                if (!predicate(label))
                {
                    continue;
                }
                using (FileStream stream = new FileStream(DataPath, FileMode.Open))
                {
                    image = new DataItem { data = ReadSingleImage(stream, i), expectedResult = label };
                }
                yield return image;
            }
        }

        private double[] ReadSingleImage(Stream stream, int index)
        {
            byte[] image = new byte[ImageSize];
            stream.Seek((index * ImageSize) + METADATA_SIZE, SeekOrigin.Begin);
            stream.Read(image, 0, image.Length);
            double[] normalizedImage = new double[image.Length];
            for (int i = 0; i < image.Length; i++)
            {
                normalizedImage[i] = (double)image[i] / MAX_PIXEL_INTENSITY;
            }
            return normalizedImage;
        }

        public int ReadInt(Stream stream)
        {
            byte[] intArray = new byte[4];
            stream.Read(intArray, 0, intArray.Length);
            for (int i = 0; i < intArray.Length/2; i++)
            {
                byte holder = intArray[i];
                intArray[i] = intArray[intArray.Length - 1 - i];
                intArray[intArray.Length - 1 - i] = holder;
            }
            
            return BitConverter.ToInt32(intArray, 0);
        }

        public int ImageLabel(string path)
        {
            int magicNumber;
            int numberOfItems;
            int label = -1;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                magicNumber = ReadInt(stream);
                numberOfItems = ReadInt(stream);
                label = stream.ReadByte();
            }
            
            return label;
        }

        public int FindLabel(int index)
        {
            int label = -1;
            using (FileStream stream = new FileStream(LabelPath, FileMode.Open))
            {
                stream.Seek(8 + index, SeekOrigin.Begin);
                label = stream.ReadByte();
            }

            return label;
        }

        public int[] ImageLabels(string path, int count)
        {
            int[] labels = null;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                ReadInt(stream);
                ReadInt(stream);
                //Skip over magic number and number of items for now
                labels = new int[count];
                for (int i = 0; i < count; i++)
                {
                    labels[i] = stream.ReadByte();
                }
            }

            return labels;
        }


        public int GetSetSize()
        {
            return NumberOfImages;
        }


        public int[] GetHiddenLayerSizes()
        {
            return new int[] { ImageSize/2};
        }

    }
}
