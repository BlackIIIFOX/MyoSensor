using System.Collections.Generic;
using System.IO;

namespace MyoSensor.Services
{
    public static class PathManager
    {
        public static readonly string MainDataPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Data\\";
        private static readonly string FileProfile = "account.json";
        private static readonly string ProfilesFolderName = "Profiles\\";
        private static readonly string SessionFolderName = "Sessions\\";
        private static readonly string LogsPath = "Logs.csv";

        public static string Txt = ".txt";
        public static string Json = ".json";

        public static List<string> GetProfilesInfoFilesPaths()
        {
            List<string> profiles = new List<string>();
            string startPath = MainDataPath + ProfilesFolderName;
            if (!Directory.Exists(startPath))
                Directory.CreateDirectory(startPath);
            string[] dirs = Directory.GetDirectories(startPath);
            foreach (var item in dirs)
            {
                string p = item + "\\" + FileProfile;
                if (File.Exists(p))
                {
                    profiles.Add(p);
                }
            }
            return profiles;
        }

        public static string GetSessionPath(int idProfile, int idSession)
        {
            string sessionFolderPath = MainDataPath + ProfilesFolderName + "\\" + idProfile.ToString() + "\\" + SessionFolderName + "\\" + idSession;
            if (!Directory.Exists(sessionFolderPath))
                Directory.CreateDirectory(sessionFolderPath);

            string sessionPath = sessionFolderPath + "\\" + LogsPath;
            if (!File.Exists(sessionPath))
            {
                File.Create(sessionPath).Close();
            }
            return sessionPath;
        }

        public static string GetProfilePath(int idProfile)
        {
            string dirPath = MainDataPath + ProfilesFolderName + idProfile.ToString() + "\\";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }
    }
}
