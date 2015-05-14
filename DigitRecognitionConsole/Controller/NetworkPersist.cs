using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DigitRecognitionConsole.Model;

namespace DigitRecognitionConsole.Controller
{
    public class NetworkPersist
    {
        public static void SaveNetwork(PersistentNetwork network, string fileName, string filePath = @"..\..\Data\")
        {
            if (fileName.ToLower() != "nofile")
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(filePath + fileName + ".ntw", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(stream, network);
                }
            }
        }

        public static PersistentNetwork LoadNetwork(string fileName, string filePath = @"..\..\Data\")
        {
            PersistentNetwork network = null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath+ fileName + ".ntw", FileMode.Open))
            {
                network = (PersistentNetwork)formatter.Deserialize(stream);
            }

            return network;
        }
    }
}
