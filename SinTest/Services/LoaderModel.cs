using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyoSensor.Services
{

    static class LoaderModel
    {

        public static void SaveSession(int idProfile, ulong idSession, List<double> data)
        {
            var csv = new StringBuilder();
            for (int i = 0; i < data.Count; i++)
            {
                var newLine = string.Format("{0};", data[i]);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(PathManager.GetSessionPath(idProfile, idSession), csv.ToString());
        }

        public static void GetProfiles()
        {
            List<string> profilesPath = PathManager.GetProfilesInfoFilesPaths();
        }

        public static List<double> LoadSession(int idProfile, ulong idSession)
        {
            string sessionPath = PathManager.GetSessionPath(idProfile, idSession);

            List<double> logsData = new List<double>();
            using (var reader = new StreamReader(sessionPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    logsData.Add(Convert.ToDouble(values[0]));
                }
            }
            return logsData;
        }

    }
}
