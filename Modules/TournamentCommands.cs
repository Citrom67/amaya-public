using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using Amaya;
using Discord;
using Amaya.Modules;
using System.Runtime.InteropServices;

namespace myBot.Modules
{
    public class tournamentCommands : ModuleBase<SocketCommandContext>
    {

        // commands

        [Command("setverifyrole")]
        [Alias("svr")]
        public async Task SetVerifyRole(SocketRole role = null)
        {
            try
            {
                if (role != null)
                {
                    List<string> roles = File.ReadAllLines(Environment.CurrentDirectory + "\\Files\\VerifyRole.txt").ToList();

                    if (roles.Exists(x => x.Contains(Context.Guild.Id.ToString())))
                    {
                        roles.RemoveAt(roles.FindIndex(x => x.Contains(Context.Guild.Id.ToString())));
                    }
                    roles.Add($"{role.Id};{Context.Guild.Id}");

                    File.WriteAllLines(Environment.CurrentDirectory + "\\Files\\VerifyRole.txt", roles.ToArray());

                    await ReplyAsync($"The verify role is set to: {role}");
                }
                else
                {
                    await ReplyAsync("You must include a role with the command!");
                }
            }
            catch (Exception ex)
            {
                Program.Api.ConsoleLoggingErrors(ex, "setverifyrole[srv]");
            }
        }

        [Command("register")]
        [Alias("reg")]
        public async Task Register([Remainder] string trash = null)
        {
            try
            {
                List<string> players = File.ReadAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt").ToList();

                if (players.Exists(x => x.Contains(Context.Message.Author.Id.ToString()))) await ReplyAsync($"Már regisztráltál!");
                else
                {
                    Global gl = new Global();

                    string name = "";
                    if (String.IsNullOrEmpty((Context.Message.Author as SocketGuildUser).Nickname))
                    {
                        name = Context.Message.Author.Username;
                    }
                    else
                    {
                        name = (Context.Message.Author as SocketGuildUser).Nickname;
                    }

                    players.Add($"{Context.Message.Author.Id};0;{gl.GetProfileAsync(name).Result.user_id};");
                    File.WriteAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt", players.ToArray());
                    await ReplyAsync("Sikeresen regisztráltál, nemsokára értesítünk a match időpontokról!");
                }
            }
            catch (Exception ex)
            {
                Program.Api.ConsoleLoggingErrors(ex, "register[reg]");
            }
        }

        [Command("verify")]
        [Alias("v")]
        public async Task VerifyMember(SocketUser user, [Remainder]string nick)
        {
            try
            {
                if (user.Username != nick)
                {
                    List<string> roles = File.ReadAllLines(Environment.CurrentDirectory + "\\Files\\VerifyRole.txt").ToList();

                    if (roles.Exists(x => x.Contains(Context.Guild.Id.ToString())))
                    {
                        await (user as SocketGuildUser).ModifyAsync(x => { x.Nickname = nick; });
                        await (user as SocketGuildUser).AddRoleAsync(Context.Guild.GetRole(ulong.Parse(roles[roles.FindIndex(x => x.Contains(Context.Guild.Id.ToString()))].Split(';').First())));
                    }
                    else
                    {
                        await ReplyAsync($"This server has not set a verify role yet! Use `{Program.Prefix}setverifyrole` or `{Program.Prefix}svr` command to set it!");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Api.ConsoleLoggingErrors(ex, "verify[v]");
            }
        }

        [Command("zaklatas")]
        public async Task Zaklatas(bool igen = false)
        {

            if (igen)
            {
                string[] lines = File.ReadAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt");
                lines[0] = "true";
                File.WriteAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt", lines);
                await ReplyAsync("Értesitők be lettek kapcsolva");
            }
            else
            {
                string[] lines = File.ReadAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt");
                lines[0] = "false";
                File.WriteAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt", lines);
                await ReplyAsync("Értesítők ki lettek kapcsolva");
            }
        
        }

        [Command("listplayers")]
        [Alias("lp", "lps")]
        public async Task ListPlayersInOrder(SocketTextChannel channel = null, [Remainder]string color = "")
        {
            try
            {
                EmbedBuilder embed = new EmbedBuilder();
                moderationCommands mc = new moderationCommands();
                Global api = new Global();
                List<string> players = File.ReadAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt").ToList();
                string content = "";
                if (channel is null)
                {
                    channel = Context.Channel as SocketTextChannel;
                }

                // Formatting the output lines and ordering the list
                for (int i = 0; i < players.Count; i++)
                {
                    players[i] = $"**{api.GetProfileAsync(players[i].Split(';')[2]).Result.username}:** *#{api.GetProfileAsync(players[i].Split(';')[2]).Result.pp_rank:N0} ({api.GetProfileAsync(players[i].Split(';')[2]).Result.pp_raw:F2}PP)*\n";
                }
                players = players.OrderByDescending(x => x.Split('#').First()).ToList();
                foreach (string line in players)
                {
                    content += line;
                }



                // Testing the color
                // Setting default values, just in case 
                int[] colors = new int[3] { 200, 0, 200 };

                //Checking if input is parseable or not
                if (color.StartsWith("[") && color.EndsWith("]") && color.Split(',').Count() == 3)
                {
                    color = color.Trim('[', ']');
                    if (int.TryParse(color.Split(',')[0], out int result))
                    {
                        colors[0] = int.Parse(color.Split(',')[0]);
                    }
                    if (int.TryParse(color.Split(',')[1], out int result1))
                    {
                        colors[1] = int.Parse(color.Split(',')[1]);
                    }
                    if (int.TryParse(color.Split(',')[2], out int result2))
                    {
                        colors[2] = int.Parse(color.Split(',')[2]);
                    }
                }
                // Checking for incorrect RGB values
                for (int i = 0; i < colors.Length; i++)
                {
                    if (colors[i] > 255)
                    {
                        colors[i] = 255;
                    }
                }

                embed.WithAuthor($"{Context.Guild.Name}", Context.Guild.IconUrl);
                embed.WithTitle("**Regisztrált Playerek:**");
                embed.WithDescription(content);
                embed.WithColor(colors[0], colors[1], colors[2]);


                await mc.DeleteMessagesAsync(0, Context.Channel);
                await channel.SendMessageAsync("", false, embed.Build());

            }
            catch (Exception ex)
            {
                Program.Api.ConsoleLoggingErrors(ex, "listplayers[lp]");
            }
        }

        // extra stuff

        public void Zaklatás()
        {
            List<string> players = File.ReadAllLines(Environment.CurrentDirectory + "\\Files\\Players.txt").ToList();

            foreach (string player in players)
            {
                // string;int;string;date
                // discordID;reminder_count;osu_username;match_date
                string[] line = player.Split(';');


                switch (int.Parse(line[1]))
                {
                    case 0:
                        Program._client.GetUser(ulong.Parse(line[0])).SendMessageAsync($"Következő matched {line[3]}-kor lesz! Gondoltam jelzek, nehogy elfelejtsd!");
                        break;
                    case 1:
                        Program._client.GetUser(ulong.Parse(line[0])).SendMessageAsync($"Következő matched {line[3]}-kor lesz! Gondoltam jelzek, nehogy elfelejtsd!");
                        break;
                    case 2:
                        Program._client.GetUser(ulong.Parse(line[0])).SendMessageAsync($"Következő matched {line[3]}-kor lesz! Gondoltam jelzek, nehogy elfelejtsd!");
                        break;
                }


            }
        }


    }
}
