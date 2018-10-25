using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyoSensor.Services;

namespace MyoSensor.Model
{
    class ProfileModel : ICloneable
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "FullName")]
        public string FullName { get; set; }

        public object Clone()
        {
            return new ProfileModel()
            {
                Id = this.Id,
                FullName = this.FullName
            };
        }

        public static List<ProfileModel> GetProfiles()
        {
            List<ProfileModel> profilesLoaded = new List<ProfileModel>();
            foreach (var item in PathManager.GetProfilesInfoFilesPaths())
            {
                profilesLoaded.Add((ProfileModel)JsonSerDer.LoadObject<ProfileModel>(item));
            }
            var profilesSorted = profilesLoaded.OrderBy(x => x.Id).ToList();
            return profilesSorted;
        }

        public static void SaveProfile(ProfileModel profile)
        {
            JsonSerDer.SaveObject(profile, PathManager.GetProfilePath(profile.Id));
        }
    }
}
