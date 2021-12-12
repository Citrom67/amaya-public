using Discord.Commands;
using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Chino_bot.Modules.Fun
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        EmbedBuilder embed = new EmbedBuilder();
        Random rnd = new Random();


        [Command("confused")]
        public async Task Confused(SocketGuildUser mention = null)
        {
            Emote emote = Emote.Parse("<:questionmark:640216663652040707>");


            //Emote.Parse("<:questionmark:640216780689637390>");
            Random rnd = new Random();
            string[] gif = new string[5] {
                "https://cdn.discordapp.com/attachments/602449119243534336/602449276798500875/confused1.gif",
                "https://cdn.discordapp.com/attachments/602449119243534336/602449277926768640/confused3.gif",
                "https://cdn.discordapp.com/attachments/602449119243534336/602449285216206858/confused4.gif",
                "https://cdn.discordapp.com/attachments/602449119243534336/602449285799346176/confused2.gif",
                "https://cdn.discordapp.com/attachments/602449119243534336/602449289108783123/confused5.gif"};

            EmbedBuilder confused = new EmbedBuilder();
            if (mention is null || mention.Mention == Context.Guild.CurrentUser.Mention)
            {
                confused.WithAuthor($"Whaaaat?", Context.Guild.CurrentUser.GetAvatarUrl());
                confused.WithDescription($"What did I do that confused you?!?"+ emote);

            }
            else
            {
                confused.WithAuthor($"Whaaaat?", Context.Guild.CurrentUser.GetAvatarUrl());
                confused.WithDescription($"{mention.Mention} confused {Context.User.Mention}" + emote);
            }

            var i = rnd.Next(0, gif.Count());
            confused.WithColor(20, 170, 255);
            confused.WithImageUrl($"{gif[i].ToString()}");
            await ReplyAsync("", false, confused.Build());
            Console.WriteLine($"{DateTime.Now}#confused@{Context.Guild.Name}");
        }

        [Command("laugh")]
        public async Task Laugh(SocketGuildUser mention = null)
        {
            Emote laught = Emote.Parse("<:laught:624903374332100619>");
            string[] laughGif = new string[] {"https://cdn.discordapp.com/attachments/602449119243534336/602461120502169601/laugh1.gif",
                                                                "https://cdn.discordapp.com/attachments/602449119243534336/602461110926573569/laugh2.gif",
                                                                "https://cdn.discordapp.com/attachments/602449119243534336/602461113203818516/laugh3.gif",
                                                                "https://cdn.discordapp.com/attachments/602449119243534336/602461112130076672/laugh4.gif",
                                                                "https://cdn.discordapp.com/attachments/602449119243534336/602461115762475031/laugh5.gif",
                                                                "https://cdn.discordapp.com/attachments/602449119243534336/602461121592557598/laugh6.gif"};

            EmbedBuilder laugh = new EmbedBuilder();
            if (mention is null || mention.Mention == Context.Guild.CurrentUser.Mention)
            {
                laugh.WithColor(20, 170, 255)
                .WithAuthor($"Heeey", Context.Guild.CurrentUser.GetAvatarUrl())
                .WithDescription($"What's so funny?!?" + laught)
                .WithImageUrl(laughGif[rnd.Next(0, laughGif.Count() + 1)]);
            }
            else
            {
                laugh.WithColor(20, 170, 255)
                .WithAuthor($"Hahaha", Context.Guild.CurrentUser.GetAvatarUrl())
                .WithDescription($"{Context.User.Mention} is laughing at {mention.Mention}" + laught);
            }
            var i = rnd.Next(0, laughGif.Count());
            laugh.WithImageUrl($"{laughGif[i]}");
            await ReplyAsync($"", false, laugh.Build());
            Console.WriteLine($"{DateTime.Now}#laugh@{Context.Guild.Name}");
        }

        [Command("avatar")]
        public async Task avatar(SocketGuildUser mention = null)
        {
            if (mention is null)
            {
                string avatarurl;
                if (Context.User.GetAvatarUrl() != null)
                {
                    avatarurl = Context.User.GetAvatarUrl(ImageFormat.Auto,256).ToString();
                }
                else
                {
                    avatarurl = "https://cdn.discordapp.com/embed/avatars/0.png";
                }
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Here you go!").WithImageUrl($"{avatarurl}");

                await ReplyAsync($"", false, builder.Build());
            }
            else
            {
                string avatarurl;
                if (mention.GetAvatarUrl() != null)
                {
                    avatarurl = mention.GetAvatarUrl(ImageFormat.Auto, 256).ToString();
                }
                else
                {
                    avatarurl = "https://cdn.discordapp.com/embed/avatars/0.png";
                }
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Here you go!").WithImageUrl($"{avatarurl}");

                await ReplyAsync($"", false, builder.Build());
            }
            Console.WriteLine($"{DateTime.Now}#avatar@{Context.Guild.Name}");
        }

        [Command("pat")]
        public async Task Headpat(SocketGuildUser user = null)
        {

            List<string> HeadPatLinks = new List<string> { "https://cdn.discordapp.com/attachments/602449119243534336/619919762717147146/headpat2.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919764793458698/headpat3.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619920232223473664/headpat.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919830627385366/headpat5.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919815116587008/headpat6.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919842945794061/headpat7.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919931462516738/headpat8.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919839632293940/headpat9.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919937317634120/headpat10.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919925774909440/headpat11.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919833705873428/headpat12.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/619919938232254514/headpat13.gif"};
            if (Context.Message.MentionedUsers.Count() == 0)
            {

                embed.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                embed.WithDescription("Ah thank you!");
                embed.WithColor(165, 200, 225);
            }
            else
            {
                embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                embed.WithDescription($"{Context.User.Mention} pats {user.Mention}'s head!");

                embed.WithColor(165, 200, 225);
            }
            var i = rnd.Next(0, HeadPatLinks.Count);
            embed.WithImageUrl($"{HeadPatLinks[i]}");
            await ReplyAsync("", false, embed.Build());
            Console.WriteLine($"{DateTime.Now}#pat@{Context.Guild.Name}");
        }

        [Command("hug")]
        public async Task Hug(SocketGuildUser user = null)
        {

            List<Emote> hugEmotes = new List<Emote>() { Emote.Parse("<:KannaHug:640228421388992515>"), Emote.Parse("<:CatHug:640228585600450561>") };

            List<string> HeadPatLinks = new List<string> {      "https://cdn.discordapp.com/attachments/602449119243534336/620220578439102494/hug1.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219890510331934/hug2.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219913738518550/hug3.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219916456296460/hug5.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219916389449739/hug4.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219920877092864/hug6.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219920889675777/hug8.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219921971806209/hug7.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219925843148820/hug10.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219926740729876/hug9.gif",
                                                                    "https://cdn.discordapp.com/attachments/602449119243534336/620219931023114250/hug11.gif"};
            if (Context.Message.MentionedUsers.Count() == 0)
            {

                embed.WithAuthor(Context.Guild.CurrentUser.Username, Context.Guild.CurrentUser.GetAvatarUrl());
                embed.WithDescription("You are so kind!" + hugEmotes[rnd.Next(0, hugEmotes.Count)]);
                embed.WithColor(165, 200, 225);
            }
            else
            {
                embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                embed.WithDescription($"{Context.User.Mention} hugs {user.Mention}!" + hugEmotes[rnd.Next(0,hugEmotes.Count)]);

                embed.WithColor(165, 200, 225);
            }
            var i = rnd.Next(0, HeadPatLinks.Count);
            embed.WithImageUrl($"{HeadPatLinks[i]}");
            await ReplyAsync("", false, embed.Build());
            Console.WriteLine($"{DateTime.Now}#hug@{Context.Guild.Name}");
        }

        public async Task BobbyUwUOwO(SocketCommandContext ThisContext, string message)
        {
            if (ThisContext.User.Id.ToString() == "238664666149027840" && message.ToLower() == "owo")
            {
                await ThisContext.Channel.SendMessageAsync("uwu");
            }
            else if (ThisContext.User.Id.ToString() == "238664666149027840" && message.ToLower() == "uwu")
            {
                await ThisContext.Channel.SendMessageAsync("owo");
            }
        }
    }
}