using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyoSensor.Services
{
    public static class FileIOManager
    {
        public static string ReadData(string path)
        {
            string str = "";
            try
            {
                using (StreamReader rdr = new StreamReader(path))
                {
                    str = rdr.ReadToEnd();
                }
                return str;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }

        }
        public static List<object> ReadBinaryData(string path, bool FloatFloat)
        {
            List<object> outer = new List<object>();
            long lenght =0, packetSize;

            packetSize = FloatFloat ? 8 : 10;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                    {
                        lenght = reader.BaseStream.Length;
                        while (lenght >= packetSize)
                        {
                            outer.Add(reader.ReadSingle());
                            if (FloatFloat)
                            {
                                outer.Add(reader.ReadSingle());
                            }
                            else
                            {
                                outer.Add(reader.ReadByte());
                                outer.Add(reader.ReadSingle());
                                outer.Add(reader.ReadByte());
                            }
                            lenght -= packetSize;
                        }
                    }
                }
            }
            return outer;
        }

        public static string WriteData(string path, string data, bool Append=false)
        {
            using (StreamWriter wrt = new StreamWriter(path, Append))
            {
                wrt.Write(data);
            }
            return data;
        }

        public static void DeleteFolder(string path)
        {
            Directory.Delete(path, true);
        }

    }
}
