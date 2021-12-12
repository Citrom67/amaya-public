using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chino_bot.Modules
{

    public class help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help(string command = null)
        {
            try
            {
                EmbedBuilder help = new EmbedBuilder();
                switch (command)
                {
                    case "osu":
                        help.WithAuthor(Context.Guild.CurrentUser.Username,Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command allows you to check your or other player's osu profile and stats(such as PP,Accuracy,etc.)\nYou can ping other users or you can write their osu username with the command. If you don't write anything then your discord username will be used!");
                        help.WithCurrentTimestamp();
                        help.WithColor(135,12,184);
                        await ReplyAsync("",false, help.Build());
                        break;
                    case "recent":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command reveals either your or the mentioned user's recent play in osu!\nYou can ping other users or write their osu username with the command. If you don't write anything then your discord username will be used!");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "compare":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command compares your or the mentioned user's play to the last beatmap that's sent by this bot!\nYou can ping other users or write their osu username after the command. If you don't write anything then your discord username will be used!");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "link":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command connects your Discord ID to a username!\n __You have to write an existing osu username with the command!__\n The benefit of linking is that you can use a different name on discord than ingame and the commands will work even if you don't write any username after commands.");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "avatar":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command sends a bigger sized picture ones avatar than you can see in discord. If you want to reveal someone's avatar just ping them. If there is no ping then the command's sender's avatar will be sent.");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "laugh":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command sends an anime laughing gif. You can ping users and it will give different results than if there is no ping.");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "confused":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command sends an anime confused gif. You can ping users and it will give different results than if there is no ping.");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "ban":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command is only available for the user's that has permission to ban members. There is a formula that the command follows.\nc.ban <mention> <hours> <reason>");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "kick":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command is only available for the user's that has permission to kick members. There is a formula that the command follows.\nc.kick <mention> <reason>");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "role":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command is only available for the user's that has permission to manage roles on the server. You need to spell th role's name correctly. There is a formula for the command.\nc.role <mention> <role name>");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "clear":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command is only available for the user's that has permission to manage messages. You can type a number with the command and that amount of chat messages will be deleted.");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "pat":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command is for to give a(n anime) head pat to one of your friend or users of server.");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "hug":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("This command is for to give a(n anime) hug to one of your friend or users of server.");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "wordban":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("With this command you can 'censor' words/phrases on your server! Just type it the word that you want to censor after the command and every time someone says it, the bot will delete the message.\nYou can remove words/phrases from the censoring with the `wordunban` command!"); 
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    case "report":
                        help.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                        help.WithDescription("You can report any problem with the me for my creator, or you can contact my creator this way! If there is any major bug/misspelling/grammatical error, please report it!\nIf something is so important then make sure to send and invite link with the report or add my creator(Citrom#9439).");
                        help.WithCurrentTimestamp();
                        help.WithColor(135, 12, 184);
                        await ReplyAsync("", false, help.Build());
                        break;
                    default: 
                            EmbedFieldBuilder osu = new EmbedFieldBuilder();
                            EmbedFieldBuilder fun = new EmbedFieldBuilder();
                            EmbedFieldBuilder useless = new EmbedFieldBuilder();
                            EmbedFieldBuilder moderaiton = new EmbedFieldBuilder();

                            osu.WithName("__**Osu related commands:**__");
                            osu.WithValue("link, stats, recent, compare");
                            osu.WithIsInline(false);

                            fun.WithName("__**Just for fun commands:**__");
                            fun.WithValue("laugh, confused, avatar, animepic, chino, pat, hug");
                            fun.WithIsInline(false);

                            moderaiton.WithName("__**Moderation commands:**__");
                            moderaiton.WithValue("kick, ban, role, clear, wordban, report");
                            moderaiton.WithIsInline(false);

                        help.WithAuthor("Keep in mind that I still work on this bot!", Context.Guild.CurrentUser.GetAvatarUrl())
                                .WithFields(moderaiton, fun, osu)
                                .WithColor(230, 230, 0)
                                .WithCurrentTimestamp()
                                .WithTitle("If you have any trouble with the bot or you found a mistake/bug/misspelling then please contact: Citrom#9439");
                            await Context.User.SendMessageAsync("", false, help.Build());
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException.Message != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                if (ex.InnerException.InnerException.Message != null)
                {
                    Console.WriteLine(ex.InnerException.InnerException.Message);
                }
            }


        }
    }
}
