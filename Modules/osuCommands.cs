using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Amaya.Modules
{
    public class osuCommands : ModuleBase<SocketCommandContext>
    {

        // Global variables
        EmbedBuilder embed = new EmbedBuilder();
        Profile osuProfile = new Profile();
        Beatmap map = new Beatmap();
        readonly Global obAPI = new Global();


        // Commands

        [Command("link")]
        [Alias("l", "linkprofile")]
        [Description("Connects one's osu name to their discord profile")]
        public async Task Link([Remainder]string username = null)
        {
            // Checking for already existing link
            List<string> linkeds = File.ReadAllLines("Files\\LinkedUsers.txt").ToList();
            if (linkeds.Any(x => x.Contains(Context.Message.Author.Id.ToString())))
            {
                linkeds.RemoveAt(linkeds.FindIndex(x => x.Contains(Context.Message.Author.Id.ToString())));
            }

            // Filling the username if left empty
            if (String.IsNullOrEmpty(username))
            {
                username = Context.Message.Author.Username;
            }

            string id = obAPI.GetProfileAsync(username).Result.user_id;

            // Checking the profile
            if (!String.IsNullOrEmpty(id))
            {   // Saving the linked user
                linkeds.Add(Context.Message.Author.Id.ToString() + ";" + id);
                File.WriteAllLines("Files\\LinkedUsers.txt", linkeds);

                // Setting the osu role on osu!Brigade
                if (Context.Guild.Id == 612233479488536587)
                {
                    await SetOsuRole(id, Context);
                }

                await ReplyAsync($"Successful Link! {Context.User.Username} - {username}");
            }
            else
            {
                await ReplyAsync("This ***isn't*** an existing account!");
            }
        }

        [Command("clearlink")]
        [Alias("cl", "clearlinkprofile")]
        [Description("Disconnects one's osu name from their discord profile")]
        public async Task ClearLink([Remainder]string username = null)
        {
            List<string> linkeds = File.ReadAllLines("Files\\LinkedUsers.txt").ToList();

            if (linkeds.Any(x => x.Contains(Context.Message.Author.Id.ToString())))
            {
                linkeds.RemoveAt(linkeds.FindIndex(x => x.Contains(Context.Message.Author.Id.ToString())));
            }

            File.WriteAllLines("Files\\LinkedUsers.txt", linkeds);

            await ReplyAsync("Successful link removal!");
        }


        [Command("osu")]
        [Alias("o", "osuprofile")]
        [Description("Shows information about someone's osu osuProfile")]
        public async Task SendOsuProfile([Remainder]string username = null)
        {
            // Variables + Object methods
            if (String.IsNullOrEmpty(username))
            {
                username = Context.Message.Author.Username;
                username = CheckLinkedUsers(username, Context.Message.Author.Id.ToString());
            }




            osuProfile = await obAPI.GetProfileAsync(username);

            // Output
            if (osuProfile != null)
            {
                embed = SetProfileDetails(osuProfile);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                await ReplyAsync("Invalid username!");
            }
        }

        [Command("recent")]
        [Alias("r", "rt")]
        public async Task SendRecentPlay([Remainder]string username = null)
        {
            // Checking user(name) in linked users list
            if (String.IsNullOrEmpty(username))
            {
                username = CheckLinkedUsers(Context.Message.Author.Username, Context.Message.Author.Id.ToString());
            }
            else if (Context.Message.MentionedUsers.Count > 0)
            {
                username = CheckLinkedUsers(Context.Message.MentionedUsers.First().Username, Context.Message.MentionedUsers.First().Id.ToString());
            }

            RecentPlay rp = await obAPI.GetRecentPlayAsync(username);
            Profile profile = await obAPI.GetProfileAsync(username);
            Beatmap beatmap = await obAPI.GetBeatmapAsync(rp.beatmap_id);

            if (rp != null)
            {
                EmbedFieldBuilder PlayInfo = new EmbedFieldBuilder();
                EmbedFieldBuilder MapInfo = new EmbedFieldBuilder();

                #region EmbedFildTexts

                string progress = "";

                if (rp.rank == "F")
                {
                    progress = $"»**Map progress:** " +
                        $"*{(((rp.count100 + rp.count300 + rp.count50 + rp.countmiss) / (double.Parse(beatmap.count_normal) + double.Parse(beatmap.count_slider) + double.Parse(beatmap.count_spinner))) * 100):F2}%*";
                }

                PlayInfo.WithName("Play information:");
                PlayInfo.WithValue(
                    $"»**Rank** *{rp.rank}* {progress}" +
                    $"\n»**Score:** *{rp.score:N0}* »Combo: {rp.maxcombo}x/{beatmap.max_combo}x" +
                    $"\n»**Accuracy:** *{100 * (((50 * rp.count50) + (100 * rp.count100) + (300 * rp.count300)) / (300 * (rp.countmiss + rp.count50 + rp.count100 + rp.count300))):F2}%* **[ {rp.count300} | {rp.count100} | {rp.count50} | {rp.countmiss} ]**" +
                    $"\n»**Mods:** *{rp.enabled_mods}*");



                MapInfo.WithName("Beatmap informations:");
                MapInfo.WithValue($"»**Length** *{beatmap.total_length}* »**Hit Length:** *{beatmap.hit_length}* »**BPM:** *{beatmap.bpm:F0}*" +
                    $"\n»**CS:** *{beatmap.diff_size}* »**AR:** *{beatmap.diff_approach}* »**OD:** *{beatmap.diff_overall}* »**HP:** *{beatmap.diff_drain}* »**Star:** *{beatmap.difficultyrating:F2}*");

                #endregion

                embed.WithAuthor($"{profile.username} | {profile.pp_raw:F2} PP | #{profile.pp_rank}", $"https://a.ppy.sh/{profile.user_id}");
                embed.WithTitle($"**{beatmap.title}**\n*By: {beatmap.artist}*" +
                    $"\n*Difficulty: {beatmap.version}*");
                embed.WithFields(PlayInfo, MapInfo);
                embed.WithThumbnailUrl($"https://b.ppy.sh/thumb/{beatmap.beatmapset_id}.jpg");

                await ReplyAsync("", false, embed.Build());

                AddToCompare(Context.Channel.Id.ToString(), beatmap.beatmap_id);

            }
        }

        [Command("compare")]
        [Alias("com", "c")]
        public async Task ComparePlay([Remainder]string username = null)
        {
            // Checking user(name) in linked users list
            #region usernameCheck
            if (String.IsNullOrEmpty(username))
            {
                username = CheckLinkedUsers(Context.Message.Author.Username, Context.Message.Author.Id.ToString());
            }
            else if (Context.Message.MentionedUsers.Count > 0)
            {
                username = CheckLinkedUsers(Context.Message.MentionedUsers.First().Username, Context.Message.MentionedUsers.First().Id.ToString());
            }
            #endregion

            string bmapID = "";

            // Reading and checking compares
            List<string> comparestxt = File.ReadAllLines("Files\\Compares.txt").ToList();
            if (comparestxt.Any(x => x.StartsWith(Context.Channel.Id.ToString())))
            {
                // Getting details   
                Profile pf = await obAPI.GetProfileAsync(username);
                bmapID = comparestxt[comparestxt.FindIndex(x => x.StartsWith(Context.Channel.Id.ToString()))].Split(';').Last();
                List<Score> cp = await obAPI.GetScoresAsync(username, bmapID);
                if (cp.Count > 0)
                {

                    Beatmap bm = await obAPI.GetBeatmapAsync(bmapID);

                    string desc = "";

                    // Embed stuff


                    foreach (Score score in cp)
                    {
                        desc += $"**{score.enabled_mods}**\n\t»***Rank:*** *{score.rank}*\n\t»***Score:*** *{score.score:N0}* »***Combo:*** {score.maxcombo}/{bm.max_combo}\n\t" +
                            $"»***Accuracy:*** *{ (100 * ((300 * score.count300) + (100 * score.count100) + (50 * score.count50))) / (300 * (score.count50 + score.count300 + score.count100 + score.countmiss)):F2}%* **[{score.count300} | {score.count100} | {score.count50} | {score.countmiss}]**\n\t" +
                            $"»***PP:*** *{double.Parse(score.pp):F2}PP*\n\t" +
                            $"»***Played at:*** *{score.date}*\n\n";
                    }
                    embed.WithTitle($"{bm.title} - {bm.artist} [{bm.version}]");
                    embed.WithDescription(desc);
                    embed.WithAuthor($"{pf.username} | {pf.pp_raw:F2}PP | #{pf.pp_rank:N0}", $"https://a.ppy.sh/{pf.user_id}");
                    embed.WithThumbnailUrl($"https://b.ppy.sh/thumb/{bm.beatmapset_id}.jpg");

                    await ReplyAsync("", false, embed.Build());
                }
                else
                {
                    await ReplyAsync($"{pf.username} has no plays on this map!");
                }
            }
            else
            {
                await ReplyAsync("There is nothing compare against in this channel!");
            }
        }


        // Additional method(s)

        private EmbedBuilder SetProfileDetails(Profile osuProfile)
        {
            embed.WithDescription($"»***Country:*** {osuProfile.country} #{osuProfile.pp_country_rank}\n" +
                $"»***Rank:*** *#{osuProfile.pp_rank:N0}*\n" +
                $"»***PP:*** *{osuProfile.pp_raw:F2}PP*\n" +
                 $"»***Playcount:*** *{osuProfile.playcount:N0}*\n" +
                 $"»***Accuracy:*** *{osuProfile.accuracy:F2}%*\n" +
                 $"»***Ranked Score:*** *{osuProfile.ranked_score:N0}*\n" +
                 $"***»Playtime:*** *{osuProfile.total_seconds_played}*")

                 .WithUrl($"https://osu.ppy.sh/users/{osuProfile.user_id}")
                 .WithTitle($"**{osuProfile.username}'s stats:**")
                 .WithThumbnailUrl($"https://a.ppy.sh/{osuProfile.user_id}");


            return embed;
        }


        private void AddToCompare(string channelID, string bmID)
        {
            List<string> compares = File.ReadAllLines("Files\\Compares.txt").ToList();
            if (compares.Any(x => x.StartsWith(channelID)))
            {
                compares[compares.FindIndex(x => x.StartsWith(channelID))] = $"{channelID};{bmID}";
            }
            else
            {
                compares.Add($"{channelID};{bmID}");
            }
            File.WriteAllLines("Files\\Compares.txt", compares.ToArray());

        }

        private string CutLinksUp(string link)
        {

            string[] linkParts = link.Split('/');

            return linkParts.Last();
        }

        private string CheckLinkedUsers(string username = null, string authorId = null)
        {

            List<string> linkedUsers = File.ReadAllLines("Files\\LinkedUsers.txt").ToList();

            if (linkedUsers.Exists(x => x.Contains(authorId)))
            {
                return linkedUsers.ElementAt(linkedUsers.FindIndex(x => x.Contains(authorId))).Split(';').Last();
            }
            else
            {
                return username;
            }
        }



        // Additional (server) functions

        public async Task SetOsuRole(string message, SocketCommandContext context)
        {

            SocketGuild OsuBrigade = context.Guild as SocketGuild;
            HttpClient client = new HttpClient();
            int RemoveNumber = 1;


            if (message.StartsWith("https://osu.ppy.sh/users/") || message.StartsWith("https://osu.ppy.sh/u/"))
            {
                RemoveNumber++;
                if (message.EndsWith("/"))
                {
                    message = message.Remove(message.Length, 1);
                }
            }

            osuProfile = await obAPI.GetProfileAsync(CutLinksUp(message));

            SocketRole role = OsuBrigade.Roles.First(x => x.Name == "Mercenary");

            if (osuProfile != null && osuProfile.country == "HU")
            {

                string[] roles = File.ReadAllLines("Files\\Roles.txt");

                for (int i = 0; i < roles.Length; i++)
                {
                    string[] parts = roles[i].Split(':');

                    if ((context.Message.Author as SocketGuildUser).Roles.Contains(OsuBrigade.Roles.First(x => x.Name == parts.First())))
                    {
                        await (context.Message.Author as SocketGuildUser).RemoveRoleAsync(OsuBrigade.Roles.First(x => x.Name == parts.First()));
                    }

                    if (osuProfile.pp_raw >= int.Parse(parts.Last()))
                    {
                        role = OsuBrigade.Roles.First(x => x.Name == parts.First());
                    }
                }

                await (context.User as IGuildUser).AddRoleAsync(role);
            }
            else
            {
                await context.Channel.SendMessageAsync("Ez a profil nem létezik, vagy nem magyar!");
                RemoveNumber++;
            }

            await Task.Delay(3000);

            IEnumerable<Discord.IMessage> messeges = await (Context.Channel as SocketTextChannel).GetMessagesAsync(RemoveNumber).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messeges);

        }

        public async Task MapAndProfileLinksInChat(SocketMessage msg, SocketCommandContext Context)
        {

            if (!msg.Author.IsBot && Context.Channel.Id != 725364427393597440)
            {

                // Map sent in chat
                if (msg.Content.StartsWith("https://osu.ppy.sh/beatmapsets/") || msg.Content.StartsWith("https://osu.ppy.sh/b/"))
                {
                    map = await obAPI.GetBeatmapAsync(CutLinksUp(msg.Content.TrimEnd('/')));

                    embed.WithThumbnailUrl($"https://b.ppy.sh/thumb/{map.beatmapset_id}.jpg")
                        .WithTitle($"{map.title} - {map.artist} [{map.version}]")
                        .WithAuthor($"{Context.User.Username} sent a map", Context.User.GetAvatarUrl())
                        .WithDescription($"**►AR:** *{map.diff_approach}* **►OD:** *{map.diff_overall}* **►HP:** *{map.diff_drain}* **►CS:** *{map.diff_size}*\n**►Star:** *{map.difficultyrating:F2}* **►BPM:** *{map.bpm}* **►Length:** *{map.total_length}*");

                    await Context.Channel.SendMessageAsync("", false, embed.Build());

                    AddToCompare(Context.Channel.Id.ToString(), map.beatmap_id);
                }
                // Profile sent in chat
                else if (msg.Content.StartsWith("https://osu.ppy.sh/users/") || msg.Content.StartsWith("https://osu.ppy.sh/u/"))
                {
                    osuProfile = await obAPI.GetProfileAsync(CutLinksUp(msg.Content.TrimEnd('/')));


                    embed = SetProfileDetails(osuProfile);


                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }

            }

        }

    }

    #region Models
    public class Beatmap
    {
        public string beatmapset_id { get; set; }
        public string beatmap_id { get; set; }
        public string approved { get; set; }
        public string total_length { get; set; }
        public string hit_length { get; set; }

        public string version { get; set; }
        public string file_md5 { get; set; }
        public double diff_size { get; set; }
        public double diff_overall { get; set; }
        public double diff_approach { get; set; }
        public double diff_drain { get; set; }
        public string mode { get; set; }
        public string count_normal { get; set; }
        public string count_slider { get; set; }
        public string count_spinner { get; set; }
        public string submit_date { get; set; }
        public string approved_date { get; set; }
        public string last_update { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public string creator { get; set; }
        public string creator_id { get; set; }
        public double bpm { get; set; }
        public string source { get; set; }
        public string tags { get; set; }
        public string genre_id { get; set; }
        public string language_id { get; set; }
        public string favourite_count { get; set; }
        public double rating { get; set; }
        public string download_unavailable { get; set; }
        public string audio_unavailable { get; set; }
        public double playcount { get; set; }
        public double passcount { get; set; }
        public int max_combo { get; set; }
        public double diff_aim { get; set; }
        public double diff_speed { get; set; }
        public double difficultyrating { get; set; }
        public double PP { get; set; }
        public string FCPP { get; set; }
    }
    public class Mods
    {
        public enum EnabledMods
        {
            NoMod = 0,
            NF = 1,
            EZ = 2,
            TD = 4,
            HD = 8,
            HR = 16,
            SD = 32,
            DT = 64,
            RX = 128,
            HT = 256,
            NC = 512,       // Only set along with DoubleTime. i.e: NC only gives 576
            FL = 1024,
            AP = 2048,
            SO = 4096,
            AT = 8192,      // Autopilot
            PF = 16384,     // Only set along with SuddenDeath. i.e: PF only gives 16416  
            K4 = 32768,
            K5 = 65536,
            K6 = 131072,
            K7 = 262144,
            K8 = 524288,
            FI = 1048576,
            RN = 2097152,
            CN = 4194304,
            TG = 8388608,
            K9 = 16777216,
            KC = 33554432,
            K1 = 67108864,
            K3 = 134217728,
            K2 = 268435456,
            Sv2 = 536870912,
            LM = 1073741824,
            KM = K1 | K2 | K3 | K4 | K5 | K6 | K7 | K8 | K9 | KC,
            FreeModAllowed = NF | EZ | HD | HR | SD | FL | FI | RX | AT | SO | KM,
            ScoreIncreaseMods = HD | HR | DT | FL | FI
        }
    }
    public class Event
    {
        public string display_html { get; set; }
        public string beatmap_id { get; set; }
        public string beatmapset_id { get; set; }
        public string date { get; set; }
        public string epicfactor { get; set; }
    }
    public class Profile
    {
        public string user_id { get; set; }
        public string username { get; set; }
        public string join_date { get; set; }
        public double count300 { get; set; }
        public double count100 { get; set; }
        public double count50 { get; set; }
        public int playcount { get; set; }
        public double ranked_score { get; set; }
        public double total_score { get; set; }
        public string pp_rank { get; set; }
        public string level { get; set; }
        public double pp_raw { get; set; }
        public double accuracy { get; set; }
        public int count_rank_ss { get; set; }
        public int count_rank_ssh { get; set; }
        public int count_rank_s { get; set; }
        public int count_rank_sh { get; set; }
        public int count_rank_a { get; set; }
        public string country { get; set; }
        public string total_seconds_played { get; set; }
        public string pp_country_rank { get; set; }
        public List<Event> events { get; set; }

    }
    public class RecentPlay
    {
        public string beatmap_id { get; set; }
        public double score { get; set; }
        public int maxcombo { get; set; }
        public double count50 { get; set; }
        public double count100 { get; set; }
        public double count300 { get; set; }
        public double countmiss { get; set; }
        public string countkatu { get; set; }
        public string countgeki { get; set; }
        public string perfect { get; set; }
        public string enabled_mods { get; set; }
        public string user_id { get; set; }
        public string date { get; set; }
        public string rank { get; set; }
        public double pp { get; set; }

    }
    public class Score
    {
        public string score_id { get; set; }
        public double score { get; set; }
        public string username { get; set; }
        public int maxcombo { get; set; }
        public double count50 { get; set; }
        public double count100 { get; set; }
        public double count300 { get; set; }
        public double countmiss { get; set; }
        public string countkatu { get; set; }
        public string countgeki { get; set; }
        public string perfect { get; set; }
        public string enabled_mods { get; set; }
        public string user_id { get; set; }
        public string date { get; set; }
        public string rank { get; set; }
        public string pp { get; set; }
        public string replay_available { get; set; }

    }
    #endregion
}
    
