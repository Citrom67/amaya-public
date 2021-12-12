using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Chino_bot.Models;
using Discord;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using OppaiSharp;
using System.Net.Http.Headers;
using osuBeatmapUtilities;
using System.Globalization;
using Discord.WebSocket;

namespace Chino_bot.Modules
{
    public class Osu : ModuleBase<SocketCommandContext>
    {
        readonly EmbedBuilder embed = new EmbedBuilder();
        readonly string k = "1643a8e2a53efbe570669c873960eb2eab8d0b7e";
        readonly string path = $"{Environment.CurrentDirectory}\\files\\linkedPlayers.txt";
        readonly string compPath = $"{Environment.CurrentDirectory}\\files\\compares.txt";


        [Command("osu")] //checked
        public async Task Profileosu([Remainder]string username = null)
        {
            HttpClient client = new HttpClient();

            //username check

            username = UsernameCheck(username);

            string url = String.Format($"http://osu.ppy.sh/api/get_user?k={k}&u={username}");

            HttpResponseMessage responseMessage = await client.GetAsync(url);

            string result = await responseMessage.Content.ReadAsStringAsync();
            IList<Profile> profile = JsonConvert.DeserializeObject<IList<Profile>>(result);
            if (profile.Count != 0)
            {   //embedbuild
                embed.WithThumbnailUrl($"https://a.ppy.sh/{profile.First().user_id}");
                embed.WithAuthor($"{profile.First().username}'s stats", $"https://a.ppy.sh/{profile.First().user_id}", $"https://osu.ppy.sh/users/{profile.First().user_id}");
                embed.WithDescription($"**Username:** *{profile.First().username}*\n**Global Rank:** *#{profile.First().pp_rank}*\n**PP:** *{profile.First().pp_raw}PP*\n**Country:** *{profile.First().country} #{profile.First().pp_country_rank}*\n**Accuray:** *{double.Parse(profile.First().accuracy):F2}%*\n**Playcount:** *{double.Parse(profile.First().playcount).ToString("N0")}*\n**Join date:** *{profile.First().join_date}*");
                embed.WithColor(154, 255, 0);
                await ReplyAsync($"", false, embed.Build());


                Console.WriteLine($"{DateTime.Now}#osu@{Context.Guild.Name}");
            }
            else
            {
                await ReplyAsync($"Sorry... Couldn't find the user {username}!");
            }
        }

        [Command("recent")] //checked
        public async Task Recentplay([Remainder]string username = null)
        {
            await Download("recent", username);
            Console.WriteLine($"{DateTime.Now}#recent@{Context.Guild.Name}");
        }

        [Command("compare")] //checked
        public async Task ComaparePlay([Remainder]string username = null)
        {
            await Download("compare", username);
            Console.WriteLine($"{DateTime.Now}#compare@{Context.Guild.Name}");
        }


        [Command("link")] //checked
        public async Task Linking([Remainder]string username = null)
        {

            string id = Context.User.Id.ToString();
            if (username is null)
            {
                username = Context.User.Username;
            }
            else if (Context.Message.MentionedUsers.Count > 0)
            {
                username = Context.Message.MentionedUsers.First().Username;
                id = Context.Message.MentionedUsers.First().Id.ToString();
            }
            List<Player> player = new List<Player>();
            List<string> lines = File.ReadAllLines(path, encoding: Encoding.UTF8).ToList();

            foreach (var line in lines)
            {
                string[] readed = line.Split(',');

                Player newPlayer = new Player
                {
                    id = readed[0],
                    osuname = readed[1]
                };

                player.Add(newPlayer);
            }

            player.Add(new Player { id = id, osuname = username });
            List<string> output = new List<string>();
            for (int i = 0; i < player.Count() - 1; i++)
            {
                if (player[i].id == id)
                {
                    player.Remove(player[i]);
                }
            }
            foreach (var Player in player)
            {
                output.Add($"{Player.id},{Player.osuname}");
            }

            File.WriteAllLines(path, output);

            await ReplyAsync($"{Context.User.Mention} is now linked to the username `{username}`");
            Console.WriteLine($"{DateTime.Now}#link@{Context.Guild.Name}");
        }

        private string UsernameCheck(string username)
        {
            List<Player> player = new List<Player>();
            List<string> lines;
            string id = "";
            if (File.Exists(path))
            {
                lines = File.ReadAllLines(path, encoding: Encoding.UTF8).ToList();

                foreach (var line in lines)
                {
                    string[] readed = line.Split(',');

                    Player newPlayer = new Player
                    {
                        id = readed[0],
                        osuname = readed[1]
                    };

                    player.Add(newPlayer);
                }
            }

            if (username == null || Context.Message.MentionedUsers.Count > 0)
            {
                if (Context.Message.MentionedUsers.Count > 0)
                {
                    id = Context.Message.MentionedUsers.First().Id.ToString();
                    username = Context.Message.MentionedUsers.First().Username;
                }
                else
                {
                    username = Context.User.Username;
                    id = Context.User.Id.ToString();
                }


                for (int i = 0; i < player.Count(); i++)
                {
                    if (player[i].id == id)
                    {
                        username = player[i].osuname;
                    }
                }
            }
            else
            {
                
            }
 


            return username;
        }
        private OppaiSharp.Mods Mod(string AddedMods)
        {
            OppaiSharp.Mods mods = OppaiSharp.Mods.NoMod;
            if (AddedMods.Contains("HD"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.Hidden;
                }
                else
                {
                    mods |= OppaiSharp.Mods.Hidden;
                }

            }
            if (AddedMods.Contains("DT"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.DoubleTime;
                }
                else
                {
                    mods |= OppaiSharp.Mods.DoubleTime;
                }

            }
            if (AddedMods.Contains("HR"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.Hardrock;
                }
                else
                {
                    mods |= OppaiSharp.Mods.Hardrock;
                }

            }
            if (AddedMods.Contains("EZ"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.Easy;
                }
                else
                {
                    mods |= OppaiSharp.Mods.Easy;
                }

            }
            if (AddedMods.Contains("NF"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.NoFail;
                }
                else
                {
                    mods |= OppaiSharp.Mods.NoFail;
                }

            }
            if (AddedMods.Contains("FL"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.Flashlight;
                }
                else
                {
                    mods |= OppaiSharp.Mods.Flashlight;
                }

            }
            if (AddedMods.Contains("NC"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.Nightcore;
                }
                else
                {
                    mods |= OppaiSharp.Mods.Nightcore;
                }

            }
            if (AddedMods.Contains("HT"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.HalfTime;
                }
                else
                {
                    mods |= OppaiSharp.Mods.HalfTime;
                }

            }
            if (AddedMods.Contains("SO"))
            {
                if (mods == OppaiSharp.Mods.NoMod)
                {
                    mods = OppaiSharp.Mods.SpunOut;
                }
                else
                {
                    mods |= OppaiSharp.Mods.SpunOut;
                }

            }
            return mods;
        }
        private string ModsConvert(double cs, double od, double hp, double ar, double bpm, double lenght, mods.Mods mods)
        {
            var individualMods = Enum
             .GetValues(typeof(mods.Mods))
             .Cast<mods.Mods>()
             .Where(mod => mods.HasFlag(mod) && mod != Models.mods.Mods.NoMod)
             .ToList();



            if (individualMods.Contains(Models.mods.Mods.DT) && individualMods.Contains(Models.mods.Mods.NC))
            {
                individualMods.Remove(Models.mods.Mods.DT);
            }
            if (individualMods.Contains(Models.mods.Mods.SD) && individualMods.Contains(Models.mods.Mods.PF))
            {
                individualMods.Remove(Models.mods.Mods.SD);
            }

            //hr
            if (individualMods.Contains(Models.mods.Mods.HR))
            {
                if ((ar * 1.4) > 10)
                {
                    ar = 10;
                }
                else
                {
                    ar *= 1.4;

                }

                if (od * 1.4 > 10)
                {
                    od = 11;
                }
                else
                {
                    od *= 1.4;
                }

                if (cs * 1.3 > 10)
                {
                    cs = 10;
                }
                else
                {
                    cs *= 1.3;
                }

                if (hp * 1.4 > 10)
                {
                    hp = 10;
                }
                else
                {
                    hp *= 1.3;
                }

            }
            //dt&nc
            if (individualMods.Contains(Models.mods.Mods.DT) || individualMods.Contains(Models.mods.Mods.NC))
            {


                if ((od * 1.185) > 11)
                {
                    od = 11;
                }
                else
                {
                    od *= 1.185;
                }

                bpm *= 1.5;
                lenght = lenght * 0.75;
                if (((ar * 2) + 13) / 3 > 11)
                {
                    ar = 11;
                }
                else
                {
                    ar = ((ar * 2) + 13) / 3;
                }

            }
            //hf
            if (individualMods.Contains(Models.mods.Mods.HT))
            {
                ar--;
                bpm *= 0.75;
                lenght *= 1.25;
            }
            //ez
            if (individualMods.Contains(Models.mods.Mods.EZ))
            {
                od *= 0.5;
                hp *= 0.5;
                cs *= 0.5;
                ar *= 0.5;

            }


            if (mods == 0)
            {
                mods = Models.mods.Mods.NoMod;
            }
            string stuff = $"{string.Join("", individualMods)}," + cs + "," + od + "," + hp + "," + ar + "," + bpm + "," + lenght;
            return stuff;
        }
        private async Task Download(string command = "recent", string username = "")
        {
            bool CRP = false;
            //Lists
            IList<RecentPlay> rp = null;
            IList<ComparedPlay> compare = null;
            IList<Models.Beatmap> beatmap = null;
            IList<Models.Profile> Profile = null;
            List<Compare> compares = null;
            List<string> channels = null;
            //EmbedFields
            EmbedFieldBuilder PlayInfo = new EmbedFieldBuilder();
            EmbedFieldBuilder MapInfo = new EmbedFieldBuilder();
            EmbedFieldBuilder Download = new EmbedFieldBuilder();
            //Strings   
            username = UsernameCheck(username); 
            string BeatMapID = null;
            string RecentURL = $"https://osu.ppy.sh/api/get_user_recent?k={k}&u={username}";
            string ProfileURL = $"https://osu.ppy.sh/api/get_user?k={k}&u={username}";
            string CompareURL = $"https://osu.ppy.sh/api/get_scores?k={k}&b={BeatMapID}&u={username}";
            string result = "";
            string Description = "";
            
            //Profile and Beatmap set
            HttpClient client = new HttpClient();
            HttpResponseMessage responseMessage;

            responseMessage = await client.GetAsync(ProfileURL);
            result = await responseMessage.Content.ReadAsStringAsync();
            Profile = JsonConvert.DeserializeObject<IList<Models.Profile>>(result);

            if (command == "recent")
            {
                responseMessage = await client.GetAsync(RecentURL);
                result = await responseMessage.Content.ReadAsStringAsync();
                rp = JsonConvert.DeserializeObject<IList<RecentPlay>>(result);
                if (rp.Count > 0)
                {
                    CRP = true;

                    string MapURL = $"https://osu.ppy.sh/api/get_beatmaps?k={k}&b={rp.First().beatmap_id}";
                    string BeatmapJson = await client.GetStringAsync(MapURL);
                    beatmap = JsonConvert.DeserializeObject<IList<Models.Beatmap>>(BeatmapJson);

                    //modconvert
                    string individualMods = ModsConvert(double.Parse(beatmap.First().diff_size)
                                                    , double.Parse(beatmap.First().diff_overall)
                                                    , double.Parse(beatmap.First().diff_drain)
                                                    , double.Parse(beatmap.First().diff_approach)
                                                    , double.Parse(beatmap.First().bpm)
                                                    , double.Parse(beatmap.First().total_length)
                                                    , (mods.Mods)int.Parse(rp.First().enabled_mods));
                    string[] details = individualMods.Split(',');
                    string AddedMods = details[0];
                    beatmap.First().diff_size = details[1];
                    beatmap.First().diff_overall = details[2];
                    beatmap.First().diff_drain = details[3];
                    beatmap.First().diff_approach = details[4];
                    beatmap.First().bpm = details[5];
                    beatmap.First().total_length = details[6];



                    double[] accData = new double[2] { ((50 * int.Parse(rp.First().count50)) + (100 * int.Parse(rp.First().count100)) + (300 * int.Parse(rp.First().count300))), 300 * (int.Parse(rp.First().count50) + int.Parse(rp.First().count100) + int.Parse(rp.First().count300) + int.Parse(rp.First().countmiss)) };
                    double accuracy = accData[0] / accData[1] * 100;


                    //create a StreamReader for your beatmap
                    byte[] data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{rp.First().beatmap_id}");
                    var stream = new MemoryStream(data, false);
                    var reader = new StreamReader(stream);


                    var beatmap2 = OppaiSharp.Beatmap.Read(reader);


                    double mapCompletion = (double.Parse(rp.First().count100) + double.Parse(rp.First().count50) + double.Parse(rp.First().count300) + double.Parse(rp.First().countmiss)) /
                                            (double.Parse(beatmap.First().count_normal) + double.Parse(beatmap.First().count_slider) + double.Parse(beatmap.First().count_spinner));

                    string complition = "";
                    EmbedFieldBuilder important = new EmbedFieldBuilder();
                    important.WithName("Informations:");
                    if (rp.First().rank == "F")
                    {
                        complition = $"***►Map Completion:*** *{mapCompletion * 100:F2}%*\n";
                    }


                    //modsetting
                    OppaiSharp.Mods mods = Mod(AddedMods);


                    //star and pp calculation
                    var diff = new DiffCalc().Calc(beatmap2, mods);
                    var pp = new PPv2(new PPv2Parameters(beatmap2, diff, accuracy: accuracy / 100, mods: mods, cMiss: int.Parse(rp.First().countmiss), combo: int.Parse(rp.First().maxcombo)));
                    var ppFC = new PPv2(new PPv2Parameters(beatmap2, diff, accuracy: accuracy / 100, mods: mods, cMiss: 0));


                    if (String.IsNullOrEmpty(AddedMods))
                    {
                        AddedMods = "Nomod";
                    }
                    //same acc FC PP
                    string iffc = "";
                    if (rp.First().perfect == "0")
                    {
                        iffc = $"({ppFC.Total:f2} if FC)";
                    }

                    
                    important.WithValue($"{complition}***►Score:*** *{double.Parse(rp.First().score).ToString("N0")}*\n***►Accuracy:*** *{accuracy:f2}%* *[{rp.First().count300}/{rp.First().count100}/{rp.First().count50}/{rp.First().countmiss}]*\n***►PP:*** *{pp.Total:f2}{iffc}*\n");
                    embed.WithTitle($"{beatmap.First().title} - {beatmap.First().artist}[{beatmap.First().version}]");
                    TimeSpan time = TimeSpan.FromSeconds(double.Parse(beatmap.First().total_length));
                    PlayInfo.WithName("Bonus informations:");
                    PlayInfo.WithValue($"***►Mods:*** *{AddedMods}*\n***►Combo:*** *{rp.First().maxcombo}x/{beatmap.First().max_combo}x*\n***►Rank:*** *{rp.First().rank}*");
                    PlayInfo.WithIsInline(true);
                    MapInfo.WithName("Map informations:");
                    MapInfo.WithValue($"***►AR:*** *{double.Parse(beatmap.First().diff_approach):F2}* ***►BPM:*** *{double.Parse(beatmap.First().bpm):F2}*\n***►HP:*** *{double.Parse(beatmap.First().diff_drain):f2}* ***►CS:*** *{double.Parse(beatmap.First().diff_size):f2}*\n***►Star:*** *{diff.Total:F2}* ***►Length:*** *{time.ToString(@"mm\:ss")}* ");
                    MapInfo.WithIsInline(true);
                    important.WithIsInline(false);
                    embed.WithFields(important,PlayInfo, MapInfo);
                    embed.WithFooter($"Mapped by: { beatmap.First().creator} | Played at: { rp.First().date}");

                    string channel_id = Context.Channel.Id.ToString();
                    compares = new List<Compare>();
                    channels = File.ReadAllLines(compPath, encoding: Encoding.UTF8).ToList();

                    foreach (var channel in channels)
                    {
                        string[] readed = channel.Split(',');

                        Models.Compare compare1 = new Compare
                        {
                            channel_id = readed[0],
                            beatmap_id = readed[1]
                        };

                        compares.Add(compare1);
                    }

                    compares.Add(new Compare { channel_id = Context.Channel.Id.ToString(), beatmap_id = beatmap.First().beatmap_id });
                    List<string> output = new List<string>();
                    for (int i = 0; i < compares.Count() - 1; i++)
                    {
                        if (compares[i].channel_id == Context.Channel.Id.ToString())
                        {
                            compares.Remove(compares[i]);
                        }
                    }
                    foreach (var line in compares)
                    {
                        output.Add($"{line.channel_id},{line.beatmap_id}");
                    }

                    File.WriteAllLines(compPath, output);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Sorry, I couldn't find any recent play with username `{username}`");
                }
            }
            else if (command == "compare")
            {
                if (File.Exists(compPath))
                {
                    compares = new List<Compare>();
                    channels = File.ReadAllLines(compPath, encoding: Encoding.UTF8).ToList();
                    foreach (var channel in channels)
                    {
                        string[] readed = channel.Split(',');
                        Compare compareplay = new Compare
                        {
                            channel_id = readed[0],
                            beatmap_id = readed[1]
                        };
                        compares.Add(compareplay);
                    }
                    BeatMapID = compares[compares.FindIndex(x => x.channel_id == Context.Channel.Id.ToString())].beatmap_id;
                }
                string MapURL = $"https://osu.ppy.sh/api/get_beatmaps?k={k}&b={BeatMapID}";
                CompareURL = $"https://osu.ppy.sh/api/get_scores?k={k}&b={BeatMapID}&u={username}";
                if (BeatMapID != null)
                {
                    responseMessage = await client.GetAsync(MapURL);
                    result = await responseMessage.Content.ReadAsStringAsync();
                    beatmap = JsonConvert.DeserializeObject<IList<Models.Beatmap>>(result);

                    responseMessage = await client.GetAsync(CompareURL);
                    result = await responseMessage.Content.ReadAsStringAsync();
                    compare = JsonConvert.DeserializeObject<IList<ComparedPlay>>(result);
                }
                TimeSpan time = TimeSpan.FromSeconds(double.Parse(beatmap.First().total_length));
                if (compare.Count > 0)
                {
                    CRP = true;
                    for (int i = 0; i < compare.Count; i++)
                    {
                        byte[] data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{BeatMapID}");
                        var stream = new MemoryStream(data, false);
                        var reader = new StreamReader(stream);
                        //modconvert
                        mods.Mods selectedMods = (mods.Mods)int.Parse(compare[i].enabled_mods);

                        string individualMods = ModsConvert(double.Parse(beatmap.First().diff_size)
                                         , double.Parse(beatmap.First().diff_overall)
                                         , double.Parse(beatmap.First().diff_drain)
                                         , double.Parse(beatmap.First().diff_approach)
                                         , double.Parse(beatmap.First().bpm)
                                         , double.Parse(beatmap.First().total_length)
                                         , (mods.Mods)int.Parse(compare[i].enabled_mods));


                        string[] details = individualMods.Split(',');
                        string AddedMods = details[0];
                        beatmap.First().diff_size = details[1];
                        beatmap.First().diff_overall = details[2];
                        beatmap.First().diff_drain = details[3];
                        beatmap.First().diff_approach = details[4];
                        beatmap.First().bpm = details[5];
                        beatmap.First().total_length = details[6];






                        double[] accData = new double[2] { ((50 * int.Parse(compare[i].count50)) + (100 * int.Parse(compare[i].count100)) + (300 * int.Parse(compare[i].count300))), 300 * (int.Parse(compare[i].count50) + int.Parse(compare[i].count100) + int.Parse(compare[i].count300) + int.Parse(compare[i].countmiss)) };
                        double accuracy = accData[0] / accData[1] * 100;


                        //read a beatmap
                        var beatmap2 = OppaiSharp.Beatmap.Read(reader);

                        //modsetting
                        OppaiSharp.Mods mods = Mod(AddedMods);
                        //star and pp calculation
                        var diff = new DiffCalc().Calc(beatmap2, mods);
                        var ppFC = new PPv2(new PPv2Parameters(beatmap2, diff, accuracy / 100, mods: mods, cMiss: 0, combo: int.Parse(beatmap.First().max_combo)));
                        var pp = new PPv2(new PPv2Parameters(beatmap2, diff, accuracy / 100, mods: mods, cMiss: int.Parse(compare[i].countmiss), combo: int.Parse(compare[i].maxcombo)));
                        double AR = double.Parse(beatmap.First().diff_approach);
                        if (AddedMods == "")
                        {
                            AddedMods = "Nomod";
                        }

                        if (compare[i].pp == null)
                        {
                            compare[i].pp = pp.Total.ToString();
                        }

                        //same acc FC PP
                        string iffc = "";
                        if (compare[i].perfect == "0")
                        {
                            iffc = $"({ppFC.Total:f2}PP if FC)";
                        }
                        embed.WithTitle($"{beatmap.First().title} - {beatmap.First().artist} [{beatmap.First().version}]");
                        Description += $"\n***►Score:*** *{double.Parse(compare[i].score).ToString("N0")}* ***►Combo:*** *{compare[i].maxcombo}/{beatmap.First().max_combo}*\n***►Mod(s):*** *{AddedMods}*  ***►Star:*** *{diff.Total:f2}*\n***►Accuracy:*** *{accuracy:f2}%* - *({compare[i].count300}/{compare[i].count100}/{compare[i].count50}/{compare[i].countmiss})*\n***►PP:*** *{double.Parse(compare[i].pp):f2}{iffc}*\n- - - - - - - - - - - - - - - - - - - - -";
                        embed.WithDescription(Description);
                        embed.WithFooter($"Mapped by: {beatmap.First().creator} | Last played at: {compare.First().date}");
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Sorry, I couldn't find any play on this map with username `{username}`");
                }
            }

            if (CRP == true)
            {
                Download.WithValue($"[Download](https://osu.ppy.sh/b/{beatmap.First().beatmap_id}/download)");
                Download.WithName("Download the beatmap:");
                Download.WithIsInline(false);
                embed.WithAuthor($"{Profile.First().username} | {Profile.First().pp_raw:F2}PP | #{Profile.First().pp_rank} | {Profile.First().country}{Profile.First().pp_country_rank}", $"https://a.ppy.sh/{Profile.First().user_id}", $"https://osu.ppy.sh/users/{Profile.First().user_id}");
                embed.WithThumbnailUrl($"https://b.ppy.sh/thumb/{beatmap.First().beatmapset_id}.jpg");
                embed.WithColor(30, 15, 70);
                embed.WithFields(Download);

                await ReplyAsync("", false, embed.Build());
            }
        }



        public async Task OsuRank(SocketMessage message = null, SocketCommandContext context = null)
        {
            if (String.IsNullOrEmpty(message.Content))
            {

            }               
        }
        public async Task LinkInChat(string message = null, SocketCommandContext context = null)
        {   
            
            if (message != null)
            {   
                HttpClient client = new HttpClient();
                if (message.StartsWith("https://osu.ppy.sh/beatmapsets/") || message.StartsWith("https://osu.ppy.sh/b/"))  
                {   
                    if (message.EndsWith("/"))
                    {
                        message = message.Remove(message.Length, 1);
                    }
                    var parts = message.Split('/'); 
                    string id = parts.Last();

                    
                    HttpResponseMessage responseMessage = await client.GetAsync($"https://osu.ppy.sh/api/get_beatmaps?k={k}&b=" +id );
                    string result = await responseMessage.Content.ReadAsStringAsync();
                    IList<Models.Beatmap> Map = JsonConvert.DeserializeObject<IList<Models.Beatmap>>(result);

                    TimeSpan time = new TimeSpan();
                    time = TimeSpan.FromSeconds(double.Parse(Map.First().total_length));

                    embed.WithAuthor($"{Map.First().title} - {Map.First().artist}");
                    embed.WithTitle($"**►Difficulty:** *{Map.First().version}*");
                    embed.WithDescription($"**►Star:** *{double.Parse(Map.First().difficultyrating):F2}* **►OD:** *{Map.First().diff_overall:f2}* **►HP:** *{Map.First().diff_drain:F2}* **►AR:** *{Map.First().diff_approach:F2}* **►CS:** *{Map.First().diff_size:F2}*\n**►BPM:** *{Map.First().bpm}* **►Length:** *{time.ToString(@"mm\:ss")}* **►Max Combo** *{Map.First().max_combo}x*");
                    
                    await context.Channel.SendMessageAsync("", false, embed.Build());

                    string channel_id = context.Channel.Id.ToString();
                    List<Compare> compares = new List<Compare>();
                    var channels = File.ReadAllLines(compPath, encoding: Encoding.UTF8).ToList();

                    foreach (var channel in channels)
                    {
                        string[] readed = channel.Split(',');

                        Models.Compare compare1 = new Compare
                        {
                            channel_id = readed[0],
                            beatmap_id = readed[1]
                        };

                        compares.Add(compare1);
                    }

                    compares.Add(new Compare { channel_id = context.Channel.Id.ToString(), beatmap_id = Map.First().beatmap_id });
                    List<string> output = new List<string>();
                    for (int i = 0; i < compares.Count() - 1; i++)
                    {
                        if (compares[i].channel_id == context.Channel.Id.ToString())
                        {
                            compares.Remove(compares[i]);
                        }
                    }
                    foreach (var line in compares)
                    {
                        output.Add($"{line.channel_id},{line.beatmap_id}");
                    }

                    File.WriteAllLines(compPath, output);
                }
                else if (message.StartsWith("https://osu.ppy.sh/users/") || message.StartsWith("https://osu.ppy.sh/u/"))
                {
                    if (message.EndsWith("/"))
                    {
                        message = message.Remove(message.Length, 1);
                    }
                    var parts = message.Split('/');
                    string id = parts.Last();
                    var responseMessage = await client.GetAsync($"https://osu.ppy.sh/api/get_user?k={k}&u="+id);
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<IList<Models.Profile>>(result);

                    embed.WithThumbnailUrl($"https://a.ppy.sh/{profile.First().user_id}");
                    embed.WithAuthor($"{profile.First().username}'s stats", $"https://a.ppy.sh/{profile.First().user_id}", $"https://osu.ppy.sh/users/{profile.First().user_id}");
                    embed.WithDescription($"**Username:** *{profile.First().username}*\n**Global Rank:** *#{profile.First().pp_rank}*\n**PP:** *{profile.First().pp_raw}PP*\n**Country:** *{profile.First().country} #{profile.First().pp_country_rank}*\n**Accuray:** *{double.Parse(profile.First().accuracy):F2}%*\n**Playcount:** *{double.Parse(profile.First().playcount).ToString("N0")}*\n**Join date:** *{profile.First().join_date}*"); embed.WithColor(154, 255, 0);
                    await context.Channel.SendMessageAsync($"", false, embed.Build());
                }
            }
        }
    }
}
