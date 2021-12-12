using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API;
using Discord.Commands;
using System.Diagnostics;
using Discord.WebSocket;
using Discord.Webhook;
using Discord;
using System.IO;

namespace Chino_bot.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>  
    {

        [Command("kick")]
        public async Task Kick(SocketGuildUser Mention = null, string reason = null)
        {
            try
            {
                if (!Context.Guild.CurrentUser.GuildPermissions.KickMembers)
                {
                    await ReplyAsync("Sorry, I don't have permission for this!");
                }
                else
                {
                    var user = Context.User as SocketGuildUser;
                    if (!(Mention is null))
                    {
                        if (user.GuildPermissions.KickMembers)
                        {
                            await Mention.KickAsync(reason);
                            await ReplyAsync($"{Mention.Username} has been kicked with a reason: {reason}");
                        }
                        else
                        {
                            await ReplyAsync("You don't have permission for this command!");
                            
                        }
                    }
                }
                Console.WriteLine($"{DateTime.Now}#kick@{Context.Guild.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Command("ban")]
        public async Task Ban(SocketGuildUser Mention = null, int hours = 1, [Remainder]string reason = null)
        {
            try
            {
                if (!Context.Guild.CurrentUser.GuildPermissions.BanMembers)
                {
                    await ReplyAsync("Sorry, I don't have permission for this!");
                }
                else
                {
                    var user = Context.User as SocketGuildUser;
                    if (!(Mention is null))
                    {
                        if (user.GuildPermissions.BanMembers)
                        {
                            await Context.Guild.AddBanAsync(Mention, (hours / 24), reason);
                            await ReplyAsync($"{Mention.Username} has been banned for {hours} hours with a reason: {reason}");
                        }
                        else
                        {
                            await ReplyAsync("You don't have permission for this command!");
                        }
                    }
                }
                Console.WriteLine($"{DateTime.Now}#ban@{Context.Guild.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }  
        }

        [Command("role")]
        public async Task Role(SocketGuildUser Mention = null, string Role = null)
        {
            try
            {
                if (!Context.Guild.CurrentUser.GuildPermissions.ManageRoles)
                {
                    await ReplyAsync("Sorry, I don't have permission for this!");
                }
                else
                {
                    var user = Context.User as SocketGuildUser;
                    if (!(Mention is null))
                    {
                        if (user.GuildPermissions.ManageRoles)
                        {
                            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == Role);
                            await Mention.AddRoleAsync(role);
                            await ReplyAsync($"{Mention.Username} now has {role} role!");
                        }
                        else
                        {
                            await ReplyAsync("You don't have permission for this command!");
                        }
                    }
                }
                Console.WriteLine($"{DateTime.Now}#role@{Context.Guild.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (!(ex.InnerException is null))
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }
        [Command("clear")]
        public async Task Clear(string StringNum = null)
        {
            try
            {
                if (!Context.Guild.CurrentUser.GuildPermissions.ManageMessages)
                {
                    await ReplyAsync("Sorry, I don't have permission for this!");
                }
                else
                {
                    var user = Context.User as SocketGuildUser;
                    if (user.GuildPermissions.ManageMessages)
                    {
                        int IntNum = 1;
                        bool intparse = int.TryParse(StringNum, out int n);
                        if (intparse)
                        {
                            IntNum = int.Parse(StringNum);
                        }

                        var messages = await Context.Channel.GetMessagesAsync(IntNum + 1).FlattenAsync();
                        await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
                    }
                    else
                    {
                        await ReplyAsync("You don't have permission for this command!");
                    }
                }
                Console.WriteLine($"{DateTime.Now}#clear@{Context.Guild.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        [Command("report")]
        public async Task Report([Remainder]string report = null)
        {
            if (string.IsNullOrEmpty(report))
            {
                await ReplyAsync("Write your problem with the command!");
            }
            else
            {
                Emote hug = Emote.Parse("<:hug:604422740333690885>");
                await Program._client.GetUser(206884438972301312).SendMessageAsync(report + $"\n\n{Context.User.Mention} - {Context.Guild.Name}");
                await Context.Channel.SendMessageAsync($"Thank you for your report {Context.User.Mention}! Your report is sent to my creator!\n" + hug);
            }
        }


        [Command("nick")]
        public async Task Nick([Remainder]string word = null)
        {
            
            if (Context.Guild.CurrentUser.GuildPermissions.ManageNicknames)
            {
                if (Context.Message.MentionedUsers.Count == 0)
                {
                    char[] startsWith = new char[] { ' ' };
                    word = word.TrimStart(startsWith);
                    await Context.Guild.GetUser(Context.User.Id).ModifyAsync(x => x.Nickname = word);
                    await ReplyAsync($"{Context.User.Username} now has a nickname `{word}`");
                }
                else
                {
                    word = word.Replace(Context.Message.MentionedUsers.First().Mention.ToString()," ");
                    char[] startsWith = new char[] {' '};
                    word = word.TrimStart(startsWith);
                    SocketUser mention = Context.Message.MentionedUsers.First();
                    await Context.Guild.GetUser(mention.Id).ModifyAsync(x => x.Nickname = word);
                    await ReplyAsync($"{mention.Username} now has a nickname `{word}`");
                }

            }
            else
            {
                await ReplyAsync("Sorry I can't change nicknames");
            }

        }

        //censoring and level system
        [Command("wordban")]
        public async Task WordBan([Remainder]string word = null)
        {
            bool volt = false;
            for (int i = 0; i < BannedWords.Count; i++)
            {
                if (Context.Guild.Id.ToString() == BannedWords[i].serverID && BannedWords[i].word == word)
                {
                    volt = true;
                }
            }


            if (volt)
            {
                await ReplyAsync("This word is already banned!");
            }
            else
            {
                var user = Context.User as SocketGuildUser;
                if (Context.Guild.CurrentUser.GuildPermissions.ManageMessages)
                {
                    if (user.GuildPermissions.ManageMessages)
                    {
                        StreamWriter writer = new StreamWriter($"{Environment.CurrentDirectory}\\files\\bannedwords.txt");
                        Models.BannedWords bannedword = new Models.BannedWords() { serverID = Context.Guild.Id.ToString(), word = word };
                        BannedWords.Add(bannedword);
                        writer.WriteLine(bannedword.serverID + ";" + bannedword.word);
                        writer.Close();
                        await ReplyAsync($"The word `{word}` is now banned on the whole server!");
                    }
                    else
                    {
                        await ReplyAsync("You don't have permission to do this!");
                    }
                }
                else
                {
                    await ReplyAsync("Sorry... I don't have permission(Manage Messages) to do this!");
                }
            }
            Console.WriteLine($"{DateTime.Now}#wordban@{Context.Guild.Name}");
        }

        [Command("wordunban")]
        public async Task UnbannedWords([Remainder]string word = null)
        {
            int ind = 0;
            bool volt = false;
            for (int i = 0; i < BannedWords.Count; i++)
            {
                if (Context.Guild.Id.ToString() == BannedWords[i].serverID && BannedWords[i].word == word)
                {
                    ind = i;
                    volt = true;
                }
            }


            if (volt)
            {
                var user = Context.User as SocketGuildUser;
                if (Context.Guild.CurrentUser.GuildPermissions.ManageMessages)
                {
                    if (user.GuildPermissions.ManageMessages)
                    {
                        List<string> lines = File.ReadAllLines($"{Environment.CurrentDirectory}\\files\\bannedwords.txt").ToList();
                        lines.RemoveAt(ind);
                        File.WriteAllLines($"{Environment.CurrentDirectory}\\files\\bannedwords.txt", lines.ToArray());
                        BannedWords.Remove(BannedWords[ind]);
                        await ReplyAsync($"The word `{word}` is now unbanned on the whole server!");
                    }
                    else
                    {
                        await ReplyAsync("You don't have permission to do this!");
                    }
                }
                else
                {
                    await ReplyAsync("Sorry... I don't have permission(Manage Messages) to do this!");
                }
            }
            Console.WriteLine($"{DateTime.Now}#wordunban@{Context.Guild.Name}");
        }

        [Command("bannedwords")]
        public async Task Bannedwords([Remainder]string anything = null)
        {
            string words = "";
            bool volt = false;
            for (int i = 0; i < BannedWords.Count; i++)
            {
                if (Context.Guild.Id.ToString() == BannedWords[i].serverID)
                {
                    words += $"**-** {BannedWords[i].word}\n";
                    volt = true;
                }
            }

            if (volt)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithAuthor(Context.Guild.CurrentUser.Username,Context.Guild.CurrentUser.GetAvatarUrl());
                embed.WithCurrentTimestamp();
                embed.WithTitle("These words are banned on this server:");
                embed.WithDescription(words);
                await ReplyAsync("", false, embed.Build());
            }
            else
            {
                await ReplyAsync("There are no banned words on this server!");
            }
            Console.WriteLine($"{DateTime.Now}#bannedwords@{Context.Guild.Name}");
        }

        [Command("status")]
        public async Task SetStatus([Remainder]string anything = null)
        {
            if (206884438972301312 == Context.User.Id)
            {
                Emote CatHug = Emote.Parse("<:CatHug:640228585600450561>");
                await Program._client.SetGameAsync(anything,"",ActivityType.Playing);
                await Context.Message.AddReactionAsync(CatHug);
            }
            else
            {
                Emote pout = Emote.Parse("<:RengePout:643577946254082060>");
                await ReplyAsync("You are not Citrom, you can't command me" + pout);
            }

            Console.WriteLine($"{DateTime.Now}#status@{Context.Guild.Name}");
        }


        static List<Chino_bot.Models.BannedWords> BannedWords = new List<Chino_bot.Models.BannedWords>();
        static List<Chino_bot.Models.Levels> levels = new List<Chino_bot.Models.Levels>();


        public async Task BannedWordsChecks(SocketMessage arg, SocketCommandContext context)
        {
            //wordcheck and remove
            SocketUserMessage msg = arg as SocketUserMessage;
            int argpos = 0;
            for (int i = 0; i < BannedWords.Count; i++)
            {
                if (context.Guild.Id.ToString() == BannedWords[i].serverID && arg.Content.ToLower().Contains(BannedWords[i].word.ToLower()) && !(arg.Author.IsBot) && !(msg.HasStringPrefix("c.", ref argpos) && !(msg.HasMentionPrefix(context.Guild.CurrentUser, ref argpos))))
                {
                    var messages = await context.Channel.GetMessagesAsync(1).FlattenAsync();
                    await (context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
                    await context.Channel.SendMessageAsync($"Your message contained a word that is not allowed on this server!");
                }
            }

        }

        public static async Task read()
        {
            // levels
            StreamReader reader;
            if (File.Exists($"{Environment.CurrentDirectory}\\files\\lvls.txt"))
            {
                reader = new StreamReader($"{Environment.CurrentDirectory}\\files\\lvls.txt");

                string line = "";
                while (!(reader.EndOfStream))
                {
                    line = await reader.ReadLineAsync();
                    string[] readed = line.Split(';');

                    Models.Levels newPlayer = new Models.Levels
                    {
                        username = readed[0],
                        userid = readed[1],
                        serverID = readed[2],
                        currentLvL = int.Parse(readed[3]),
                        currentXP = double.Parse(readed[4]),
                        nextLvL = double.Parse(readed[5]),
                        status  = readed[6]
                    };

                    levels.Add(newPlayer);
                }
                Console.WriteLine("Levels:\t\tReady");
                reader.Close();
            }
            else
            {
                File.Create(Environment.CurrentDirectory + $"\\files\\lvls.txt");
            }

            // bannedwords
            if (File.Exists($"{Environment.CurrentDirectory}\\files\\bannedwords.txt"))
            {
                reader = new StreamReader($"{Environment.CurrentDirectory}\\files\\bannedwords.txt");

                string line = "";
                while(!(reader.EndOfStream))
                {
                    line = await reader.ReadLineAsync();
                    string[] readed = line.Split(';');

                    Models.BannedWords newPlayer = new Models.BannedWords
                    {
                        serverID = readed[0],
                        word = readed[1]
                    };

                    BannedWords.Add(newPlayer);
                }
                Console.WriteLine("Banned words:\t\tReady");
                reader.Close();
            }
            else
            {
                File.Create($"{Environment.CurrentDirectory}\\files\\bannedwords.txt");
            }
            
        }
    }
}
