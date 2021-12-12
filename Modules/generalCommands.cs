using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Amaya.Modules
{
    public class generalCommands : ModuleBase<SocketCommandContext>
    {

        // Variables
        readonly Random rng = new Random();
        readonly EmbedBuilder embed = new EmbedBuilder();


        // Commands

        [Command("8ball")]
        [Alias("8b")]
        public async Task ShakeEihghtBall([Remainder] string question = null)
        {
            if (question.EndsWith("?"))
            {
                List<string> answers = new List<string>() { "It is certain.",
                                                        "It is decidedly so.",
                                                        "Without a doubt.",
                                                        "Yes – definitely.",
                                                        "You may rely on it.",
                                                        "As I see it, yes.",
                                                        "Most likely.",
                                                        "Outlook good.",
                                                        "Yes.",
                                                        "Signs point to yes.",
                                                        "Reply hazy, try again.",
                                                        "Ask again later.",
                                                        "Better not tell you now.",
                                                        "Cannot predict now.",
                                                        "Concentrate and ask again.",
                                                        "Don't count on it.",
                                                        "My reply is no.",
                                                        "My sources say no.",
                                                        "Outlook not so good.",
                                                        "Very doubtful." };

                await ReplyAsync("**The magic 8-ball says:** *" + answers[rng.Next(answers.Count)] + "*");
            }
            else
            {
                await ReplyAsync("It's not a question!");
            }


        }

        [Command("roll")]
        public async Task Roll([Remainder] string tryMaxNumber = null)
        {
            int maxNumber;
            if (int.TryParse(tryMaxNumber, out _) && tryMaxNumber is object)
            {
                maxNumber = int.Parse(tryMaxNumber) + 1;
            }
            else
            {
                maxNumber = 101;
            }

            await ReplyAsync($"You rolled: {rng.Next(maxNumber)}");

        }

        [Command("avatar")]
        [Alias("a", "av")]
        public async Task SendAvatar(SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }
            if (user.Id == 780537114395672616)
            {
                embed.WithTitle("My avatar is made by **Bandi**").WithDescription("His pixiv: https://www.pixiv.net/en/users/26944452");
            }

            embed.WithImageUrl(user.GetAvatarUrl(ImageFormat.Auto, 1024))
                .WithAuthor(Context.Client.CurrentUser.Username, Context.Client.CurrentUser.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor((user as SocketGuildUser).Roles.OrderByDescending(x => x.Position).First().Color);

            await ReplyAsync("", false, embed.Build());


        }

        [Command("report")]
        public async Task ReportIssue([Remainder] string message = null)
        {
            if (message.Length > 5)
            {
                await Program._client.GetUser(206884438972301312).SendMessageAsync($"**New report:** {DateTime.Now:HH:mm:ss}\n\n{message}");
                Emote Ayaya = Emote.Parse("<:Ayaya:699542498250326028>");
                await ReplyAsync("Thanks for your report! In the next update your issue hopefully will be fixed! " + Ayaya);
            }
            else
            {
                await ReplyAsync("Your report has not been sent! You need to write about your problem!");
            }
        } 
        
        [Command("reload")]
        public async Task ReloadConfig([Remainder] string trash = null)
        {
            if (Context.Message.Author.Id == 751856140090081300)
            {
                Program.GetStartingConfig();
                await Program._client.SetGameAsync(Program.Status, "", ActivityType.Playing);

                await ReplyAsync("My config has been reloaded");
            }
        }

        #region Gif_Commands
        [Command("fuck")]
        public async Task SendFuck(SocketUser user = null)
        {

            if ((Context.Channel as SocketTextChannel).IsNsfw)
            {
                Emote orgasm = Emote.Parse("<:orgasm:643577947399389194>");
                Emote RengePout = Emote.Parse("<:RengePout:643577946254082060>");
                switch (user)
                {
                    case null:
                        await ReplyAsync("I don't know much about sex, but I think at least 2 people are needed for it!" + RengePout);

                        break;
                    default:
                        embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                        embed.WithTitle($"{Context.User.Username} fucks {user.Username}! " + orgasm);
                        embed.WithColor((user as SocketGuildUser).Roles.OrderByDescending(x => x.Position).First().Color);
                        embed.WithImageUrl(GetRandomURL("fuck"));


                        await ReplyAsync("", false, embed.Build());
                        break;
                }
            }
            else
            {
                await ReplyAsync("This command is only available in NSFW channels!");
            }

        }

        [Command("nom")]
        public async Task SendNom(SocketUser user = null)
        {
            Emote DX = Emote.Parse("<:DX:699542498158182413>");
            Emote owo = Emote.Parse("<:owo:649636855285022741>");
            switch (user)
            {
                case null:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"DON'T NOM MEEEEEEEEEE!!!!!! " + DX);

                    break;
                default:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"{Context.User.Username} is nomming {user.Username}! " + owo);
                    break;
            }

            embed.WithColor((user as SocketGuildUser).Roles.OrderByDescending(x => x.Position).First().Color);
            embed.WithImageUrl(GetRandomURL("nom"));
            await ReplyAsync("", false, embed.Build());

        }

        [Command("laugh")]
        public async Task SendLaugh(SocketUser user = null)
        {
            Emote laught = Emote.Parse("<:laught:624903374332100619>");
            switch (user)
            {
                case null:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"{Context.User.Username} is laughing! " + laught);

                    break;
                default:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"{Context.User.Username} is laughing at {user.Username}! " + laught);
                    break;
            }

            embed.WithColor((user as SocketGuildUser).Roles.OrderByDescending(x => x.Position).First().Color);
            embed.WithImageUrl(GetRandomURL("laugh"));
            await ReplyAsync("", false, embed.Build());

        }

        [Command("kiss")]
        public async Task SendKiss(SocketUser user = null)
        {
            Emote luv = Emote.Parse("<:luv:624903373895761931>");
            Emote RengePout = Emote.Parse("<:RengePout:643577946254082060>");
            switch (user)
            {
                case null:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"{Context.User.Username} you can't kiss me! Grrr~~" + RengePout);

                    break;
                default:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"{Context.User.Username} is kissing {user.Username}! How romantic!" + luv);
                    embed.WithImageUrl(GetRandomURL("kiss"));
                    break;
            }

            embed.WithColor((user as SocketGuildUser).Roles.OrderByDescending(x => x.Position).First().Color);

            await ReplyAsync("", false, embed.Build());

        }

        [Command("hug")]
        public async Task SendHug(SocketUser user = null)
        {
            Emote CatHug = Emote.Parse("<:CatHug:640228585600450561>");
            switch (user)
            {
                case null:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"There there, {Context.User.Username}! Everyone needs a hug sometimes! " + CatHug);

                    break;
                default:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"{Context.User.Username} is hugging {user.Username}! Cute~ " + CatHug);
                    break;
            }

            embed.WithColor((user as SocketGuildUser).Roles.OrderByDescending(x => x.Position).First().Color);
            embed.WithImageUrl(GetRandomURL("hug"));
            await ReplyAsync("", false, embed.Build());

        }

        [Command("pat")]
        public async Task PatCommand(SocketUser user = null)
        {
            Emote questionmark = Emote.Parse("<:questionmark:624903374386626563>");
            Emote uwu = Emote.Parse("<:uwu:699542925985710140>");
            switch (user)
            {
                case null:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"Who allowed you to pat me..? " + questionmark);

                    break;
                default:
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithTitle($"{Context.User.Username} is patting {user.Username}! " + uwu);
                    break;
            }

            embed.WithColor((user as SocketGuildUser).Roles.OrderByDescending(x => x.Position).First().Color);
            embed.WithImageUrl(GetRandomURL("pat"));
            await ReplyAsync("", false, embed.Build());

        }
        #endregion

        // Methods

        private string GetRandomURL(string whatIs)
        {
            string[] URLS = new string[0];
            switch (whatIs)
            {
                case "fuck":
                    URLS = new string[] {   "https://media.discordapp.net/attachments/602449119243534336/742494272535658516/fuck1.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/742494293335212102/fuck2.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/742494308225253496/fuck3.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/742494326986113085/fuck5.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/742494339564830811/fuck6.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/742494350998634576/fuck7.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/742494363036287207/fuck8.gif"};
                    break;
                case "nom":
                    URLS = new string[] {   "https://media.discordapp.net/attachments/602449119243534336/741274696078852156/nom1.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/741274724134289408/nom2.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/741274741234466846/nom3.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/741274746553106462/nom4.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/741274752487915540/nom5.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/741274761916579981/nom6.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/741274825775120434/nom8.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/741274952464072764/nom9.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/742494117329633360/nom10.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746014577522638988/nom11.gif"};
                    break;
                case "laugh":
                    URLS = new string[] {   "https://media.discordapp.net/attachments/602449119243534336/602461120502169601/laugh1.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/602461110926573569/laugh2.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/602461113203818516/laugh3.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/602461112130076672/laugh4.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/602461115762475031/laugh5.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/602461121592557598/laugh6.gif"};
                    break;
                case "pat":
                    URLS = new string[] {   "https://media.discordapp.net/attachments/602449119243534336/619920232223473664/headpat.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919762717147146/headpat2.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919764793458698/headpat3.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919830627385366/headpat5.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919815116587008/headpat6.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919842945794061/headpat7.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919931462516738/headpat8.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919839632293940/headpat9.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919937317634120/headpat10.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919925774909440/headpat11.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919833705873428/headpat12.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/619919938232254514/headpat13.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620998481791680516/pat10.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013505580433548/headpat15.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013511083360304/headpat16.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746014034582700172/headpat17.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013514468163625/headpat18.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013488316547143/headpat19.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013494360408194/headpat20.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013498613563392/headpat21.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013500626698260/headpat22.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013502912725002/headpat23.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013503919489184/headpat24.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/746013506503180318/headpat25.gif"
                    };
                    break;
                case "kiss":
                    URLS = new string[] {   "https://media.discordapp.net/attachments/602449119243534336/629716980144013312/kiss1.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/629716984627724311/kiss2.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/629716986989117440/kiss3.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/629716990122000394/kiss4.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/629716989082075157/kiss5.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/629716993070727170/kiss6.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/629716993825832980/kiss7.gif"};
                    break;
                case "hug":
                    URLS = new string[] {   "https://media.discordapp.net/attachments/602449119243534336/620220578439102494/hug1.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219890510331934/hug2.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219913738518550/hug3.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219916389449739/hug4.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219916456296460/hug5.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219920877092864/hug6.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219921971806209/hug7.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219920889675777/hug8.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219926740729876/hug9.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219925843148820/hug10.gif",
                                            "https://media.discordapp.net/attachments/602449119243534336/620219931023114250/hug11.gif"};
                    break;
            }



            return URLS[rng.Next(0, URLS.Length + 1)];
        }

    }
}
