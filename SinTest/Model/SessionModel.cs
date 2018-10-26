using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyoSensor.Services;
using System.Collections.ObjectModel;

namespace MyoSensor.Model
{
    class SessionModel
    {

        [JsonProperty(PropertyName = "idSession")]
        public ulong Id { get; set; }
        [JsonProperty(PropertyName = "DataSession")]
        public string DateSession { get; set; }
        [JsonProperty(PropertyName = "Comment")]
        public string Сomment { get; set; }


        public static ObservableCollection<SessionModel> GetSessions(int profileId)
        {
            ObservableCollection<SessionModel> sessionLoaded = new ObservableCollection<SessionModel>();
            foreach (var item in PathManager.GetSessionsInfoFilesPaths(profileId))
            {
                sessionLoaded.Add((SessionModel)JsonSerDer.LoadObject<SessionModel>(item));
            }
            // var profilesSorted = sessionLoaded.OrderBy(x => x.Id).ToList();
            return sessionLoaded;
        }

        public static void SaveSession(int profileId, SessionModel session)
        {
            JsonSerDer.SaveObject(session, PathManager.GetSessionInfoPath(profileId, session.Id));
        }

    }
}
