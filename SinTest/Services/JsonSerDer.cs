using Newtonsoft.Json;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyoSensor.Services
{
    public static class JsonSerDer
    {
        public static bool SaveObject(object obj, string path)
        {
            try
            {
                FileIOManager.WriteData(path, JsonConvert.SerializeObject(obj));
                return true;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return false;
            }
        }

        public static object LoadObject<T>(string path)
        {
            try
            {
                if (FileIOManager.ReadData(path) == "")
                {
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(FileIOManager.ReadData(path));
                }                
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }

        }

        public static List<DataPoint> GetDataFromFile(string path)
        {
            List<DataPoint> result = new List<DataPoint>();            
            try
            {
                string data = FileIOManager.ReadData(path);
                if (data != "")
                {
                    foreach (var dataLine in data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        if (dataLine != "")
                        {
                            DataPoint ar = ParseData(dataLine);
                            result.Add(ar);
                        }  
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
            return result;
        }

        private static DataPoint ParseData(string data)
        {
            string[] parts = data.Split(':');
            return new DataPoint(float.Parse(parts[0]), float.Parse(parts[1]));
        }

    }
}
