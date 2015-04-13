using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class DigitDataReader : IDataProvider
    {
        private readonly int NUM_OF_DIGITS = 10;
        private string DataPath;
        private string LabelPath;
        private int index = 0;
        private int NumberOfImages;
        public int DataSize { get; set; }

        public DigitDataReader(string DataPath, string LabelPath)
        {
            this.DataPath = DataPath;
            this.LabelPath = LabelPath;
            SetImageSize();
        }

        private void SetImageSize()
        {
            using (FileStream stream = new FileStream(DataPath, FileMode.Open))
            {
                NumberOfImages = ReadInt(stream);
                int row = ReadInt(stream);
                int col = ReadInt(stream);
                this.DataSize = row * col;
            }
        }

        public int[] GetPossibleOutputs()
        {
            int[] numbers = new int[NUM_OF_DIGITS];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = i;
            }
            return numbers;
        }

        public int GetNumOfInputs()
        {
            return DataSize;
        }

        public DataItem GetNextDataItem()
        {
            DataItem image;

            using (FileStream stream = new FileStream(DataPath, FileMode.Open))
            { 
                stream.Seek((index * DataSize) + 16, SeekOrigin.Begin);
                image = new DataItem { data = ReadSingleImage(stream), expectedResult = FindLabel(index)};
                index++;
            }

            return image;
        }

        private byte[] ReadSingleImage(Stream stream)
        {
            byte[] image = new byte[DataSize];
            stream.Read(image, 0, image.Length);
            return image;
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


        public int GetTrainingSetSize()
        {
            return NumberOfImages;
        }
    }
}
