using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DigitRecognitionDisplay.Model;

namespace DigitRecognitionDisplay.Controller
{
    public class NetworkPersist
    {
        private static readonly string FILE_PATH = @"..\..\Data\";

        public static void SaveNetwork(PersistentNetwork network, string fileName)
        {
            if (fileName.ToLower() != "nofile")
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(FILE_PATH + fileName + ".ntw", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(stream, network);
                }
            }
        }

        public static PersistentNetwork LoadNetwork(string fileName)
        {
            PersistentNetwork network = null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(FILE_PATH + fileName + ".ntw", FileMode.Open))
            {
                network = (PersistentNetwork)formatter.Deserialize(stream);
            }

            return network;
        }
    }
}
