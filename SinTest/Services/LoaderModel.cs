using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyoSensor.Services
{

    static class LoaderModel
    {

        public static void SaveSession(int idProfile, ulong idSession, List<List<double>> data)
        {
            var csv = new StringBuilder();
            for (int i = 0; i < data[0].Count; i++)
            {
                var newLine = string.Format("{0},{1};", data[0][i], data[1][i]);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(PathManager.GetSessionPath(idProfile, idSession), csv.ToString());
        }

        public static void GetProfiles()
        {
            List<string> profilesPath = PathManager.GetProfilesInfoFilesPaths();
        }

        public static List<List<double>> LoadSession(int idProfile, ulong idSession)
        {
            string sessionPath = PathManager.GetSessionPath(idProfile, idSession);
            List<List<double>> logsData = new List<List<double>>();

            List<double> logsChannel1 = new List<double>();
            List<double> logsChannel2 = new List<double>();
            using (var reader = new StreamReader(sessionPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';',',');
                    logsChannel1.Add(Convert.ToDouble(values[0]));
                    if (values.Length > 2)
                    {
                        logsChannel2.Add(Convert.ToDouble(values[1]));
                    }

                }
            }
            logsData.Add(logsChannel1);
            logsData.Add(logsChannel2);
            // logsData.Add(Convert.ToDouble(values[0]));
            return logsData;
        }

    }
}
