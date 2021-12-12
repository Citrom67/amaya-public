using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Amaya.Modules
{
    public class Global
    {
        readonly string BaseURL = "https://osu.ppy.sh/api/";


        // osu!

        #region Profile
        public async Task<Profile> GetProfileAsync(string username)
        {



            HttpResponseMessage responseMessage = await Program.HttpClient.GetAsync(BaseURL + $"get_user?k={Program.OsuKey}&u={username}");
            string StringMessage = await responseMessage.Content.ReadAsStringAsync();


            List<Profile> profile = JsonConvert.DeserializeObject<List<Profile>>(StringMessage);

            if (profile.Count > 0)
            {
                TimeSpan totalTime = TimeSpan.FromSeconds(int.Parse(profile.FirstOrDefault().total_seconds_played));

                profile.First().total_seconds_played = $"{totalTime.TotalDays:F1}d ({totalTime.TotalHours:F0}h)";

                return profile.First();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Beatmap

        public async Task<Beatmap> GetBeatmapAsync(string beatmapID)
        {
            HttpResponseMessage responseMessage = await Program.HttpClient.GetAsync(BaseURL + $"get_beatmaps?k={Program.OsuKey}&b={beatmapID}");
            string StringResponseMessage = await responseMessage.Content.ReadAsStringAsync();

            List<Beatmap> beatmap = JsonConvert.DeserializeObject<List<Beatmap>>(StringResponseMessage);

            TimeSpan total = TimeSpan.FromSeconds(int.Parse(beatmap.First().total_length));
            TimeSpan hit = TimeSpan.FromSeconds(int.Parse(beatmap.First().hit_length));

            beatmap.First().hit_length = hit.Minutes + ":" + hit.Seconds;
            beatmap.First().total_length = total.Minutes + ":" + total.Seconds;

            if (beatmap.Count() > 0)
            {
                return beatmap.First();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Recent_Play

        public async Task<RecentPlay> GetRecentPlayAsync(string username)
        {
            HttpResponseMessage responseMessage = await Program.HttpClient.GetAsync(BaseURL + $"get_user_recent?k={Program.OsuKey}&u={username}");
            string StringResponseMessage = await responseMessage.Content.ReadAsStringAsync();

            List<RecentPlay> RecentPlays = JsonConvert.DeserializeObject<List<RecentPlay>>(StringResponseMessage);

            RecentPlays.First().enabled_mods = ConvertMods(int.Parse(RecentPlays.First().enabled_mods));

            if (RecentPlays.Count() > 0)
            {
                return RecentPlays.First();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Compare

        public async Task<List<Score>> GetScoresAsync(string username, string bmapID)
        {
            HttpResponseMessage responseMessage = await Program.HttpClient.GetAsync(BaseURL + $"get_scores?k={Program.OsuKey}&b={bmapID}&u={username}");
            string StringResponseMessage = await responseMessage.Content.ReadAsStringAsync();

            List<Score> compares = JsonConvert.DeserializeObject<List<Score>>(StringResponseMessage);

           

            foreach (Score score in compares)
            {
                if (score.pp == null)
                {
                    score.pp = "0.00";
                }    
                score.enabled_mods = ConvertMods(int.Parse(score.enabled_mods));
            }

            return compares;
        }
        #endregion

        protected string ConvertMods(int EnabledMods)
        {

            Mods.EnabledMods selectedMods = (Mods.EnabledMods)EnabledMods;

            List<Mods.EnabledMods> individualMods = Enum
                .GetValues(typeof(Mods.EnabledMods))
                .Cast<Mods.EnabledMods>()
                .Where(mod => selectedMods.HasFlag(mod) && mod != Mods.EnabledMods.NoMod)
                .ToList();

            string mods = "";

            for (int i = 0; i < individualMods.Count; i++)
            {
                mods += individualMods[i].ToString() + "+";
            }

            mods = mods.TrimEnd('+');

            if (String.IsNullOrEmpty(mods))
            {
                mods = "NoMod";
            }

            return mods;
        }

       // General

        public void ConsoleLoggingErrors(Exception ex, string Command)
        {
            Console.WriteLine(
                $"\n\n----------------------------------\n" +
                $"{Command} ran into a problem:\n" +
                $"Source:\t{ex.Source}\n" +
                $"Message:\n{ex.Message}\n" +
                $"Inner Exception's message:{ex.InnerException.Message}\n" +
                $"----------------------------------\n\n");
        }


    }
}
