using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.ComponentModel;
using Discord.WebSocket;
using System.Runtime.Remoting.Contexts;
using Amaya;

namespace Amaya.Modules
{
    public class moderationCommands : ModuleBase<SocketCommandContext>
    {
        // Variables
        EmbedBuilder embed = new EmbedBuilder();

        // Commands

        [Command("status")]
        public async Task Status([Remainder] string status)
        {
            if (status != null)
            {
                await Program._client.SetGameAsync(status,"",ActivityType.Listening);
            }
        }

        [Command("kick")]
        [Description("Kicks a given person")]
        public async Task kickMembers(SocketGuildUser mention = null, [Remainder]string reason = "")
        {
            if (mention != null)
            {
                await mention.KickAsync(reason);
                embed.WithAuthor(Context.Client.CurrentUser.Username, Context.Client.CurrentUser.GetAvatarUrl());
                embed.WithTitle($"{mention.Username} has been kicked by {Context.User.Username}");
                embed.WithDescription($"**Reason:**\n{reason}");
                embed.WithCurrentTimestamp();

                await ReplyAsync("", false, embed.Build());
            }
            else
            {
                await ReplyAsync("You need to mention the user you want to kick!");
            }
        }

        [Command("ban")]
        [Description("Kicks a given person")]
        public async Task banMembers(SocketGuildUser mention = null, [Remainder]string reason = "")
        {
            if (mention != null)
            {
                await mention.BanAsync(7, reason);
                embed.WithAuthor(Context.Client.CurrentUser.Username, Context.Client.CurrentUser.GetAvatarUrl());
                embed.WithTitle($"{mention.Username} has been banned by {Context.User.Username}");
                embed.WithDescription($"**Reason:**\n{reason}\n**Duration:**\n1 week");
                embed.WithCurrentTimestamp();

                await ReplyAsync("", false, embed.Build());
            }
            else
            {
                await ReplyAsync("You need to mention the user you want to ban!");
            }
        }

        [Command("clear")]
        [Alias("cm")]
        [Description("Clears a specific amount of messages, default amount is 1")]
        public async Task clearMessages([Remainder]string tryNumber = "1")
        {
            int number = 1;
            if (int.TryParse(tryNumber, out number))
            {
                number = int.Parse(tryNumber);
            }

            await DeleteMessagesAsync(number, Context.Channel);
        }


        // Methods

        public async Task DeleteMessagesAsync(int number, ISocketMessageChannel Channel)
        {
            IEnumerable<IMessage> messages = await Channel.GetMessagesAsync(number + 1).FlattenAsync();
            await (Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }
    }
}
