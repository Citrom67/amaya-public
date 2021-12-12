using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Amaya;

namespace Amaya.Modules
{
    public class helpCommand :ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Description("Lists all the commands")]
        [RequireBotPermission(GuildPermission.SendMessages, ErrorMessage = "Sorry I can't send messages")]
        public async Task help([Remainder] string option = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            string p = Program.Prefix;

            switch (option)
            {
                case "admin":
                    embed.WithAuthor("Here are my commands that are useable by admins");
                    embed.WithDescription($"**►kick:**\n*Kicks someone from the server*\n*Exapmle: `{p}kick <mention>, reason`*" +
                        $"\n**►ban:**\n*Bans a someone from the server for 1 week*\n*Example: `{p}kick <mention>, reason`*" +
                        $"\n**►clear:**\n*Deletes messages from the given channel. Can only delete messages that are not yet 2 weeks old!*\n*Example: `{p}clear 10`*");
                    break;

                case "rpg":
                    embed.WithAuthor("Here are the commands for my RPG section");
                    embed.WithTitle("Note that this section is far from finished! So expect bugs, not fully working features and a lot of resets!");
                    embed.WithDescription($"       **__[PROFILE]__**\n" +
                        $"**►createprofile [cp]**\n*This command creates your RPG profile.*\n" +
                        $"**►deleteprofile [dp]**\n*You can delete your profile with this command, but note that if you want to play again, you'll need to create a new profile*\n" +
                        $"**►profile [p]**\n*It displays the stats of your profile, such as hp, armor etc. (inventory will be added later)*\n" +
                        $"       **__[COMBAT]__**\n" +
                        $"**►attack [at]**\n*It allows you to attack an enemy and also if you are not in fight, this command will start the fight*\n" +
                        $"**►escape [e]**\n*You have one chance to escape from any enemy if the fight doesn't turned out as you wanted to*\n" +
                        $"**►heal [h]**\n*Consumes a healing potion, if you have any and restores hp during fights*\n" +
                        $"       **__[TRAVEL]__**\n" +
                        $"**►travel [t]**\n*Allows you to travel around  the world*\n" +
                        $"**►currentfloor [cf]**\nLets you see what areas are on your current floor, also displays the common enemy type of the floor");
                    break;
                default:
                    embed.WithAuthor($"Here are my commands that are useable by all users");
                    embed.WithDescription($"**►avatar [a]**\n*Displays a server member's discord avatar.*\n" +
                        $"**►8ball [8b]**\n*Answers a yes-or-no question, like the (once) popular Magic 8-Ball.*\n" +
                        $"**►weather [w]**\n*Shows the current weather to a specific city.*\n" +
                        $"**►roll**\n*Rolls a number between 0 and 100 or your max number.*\nExamples: `{p}roll` `{p}roll 750`\n\n" +
                        $"__**Reaction commands:**__" +
                        $"\n**►pat:**\n*Virtually pats someone.*" +
                        $"\n**►laugh:**\n*Virtually laughs at someone.*" +
                        $"\n**►hug:**\n*Virutally hugs someone.*" +
                        $"\n**►kiss:**\n*Virtually kisses someone.*" +
                        $"\n**►nom:**\n*Virtually noms someone.*" +
                        $"\n**►fuck:**\n*Virtually fucks someone. (Only works in nsfw channels)*" +
                        $"\n\n__**osu! commands:**__" +
                        $"\n**►osu [o]:**\n*Displays details about someone's osu profile.*\n*Examples: `{p}osu` `{p}o` `{p}o Rafis`*" + 
                        $"\n**►link [l]:**\n*Links your osu username to your account*\n*Examples: `{p}link {Context.Client.CurrentUser.Username}` `{p}l {Context.Client.CurrentUser.Username}`*" +
                        $"\n\nIf you have any problems, or found a bug/misspelling, please use the `{p}report` command!");
                    embed.WithFooter($"Administrator commands at: `{p}help admin`");
                    break;               
            }
            await ReplyAsync("",false, embed.Build());
        }
    }
}
