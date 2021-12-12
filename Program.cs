using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Amaya.Modules;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Amaya
{
    class Program : ModuleBase<SocketCommandContext>
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public static string OsuKey;
        public static string DcKey;
        public static string Prefix;
        public static string Status;
        public static Global Api = new Global();
        public static HttpClient HttpClient = new HttpClient();

        public static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        // Startup
        public static void GetStartingConfig()
        {
            List<string> lines = File.ReadAllLines("Config.ini").ToList();

            DcKey = lines[0].Split(':').Last().Trim(' ');
            OsuKey = lines[1].Split(':').Last().Trim(' ');
            Prefix = lines[2].Split(':').Last().Trim(' ');
            Status = lines[3].Split(':').Last().TrimStart(' ').TrimEnd(' ');

        }

        public async Task MainAsync()
        {
            GetStartingConfig();
            Console.Title = "Amaya Discord Bot";

            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += _client_Log;

            
            await _client.SetStatusAsync(status: UserStatus.Online);
            await _client.SetGameAsync(Status, "", ActivityType.Playing);
            await RegisterCommands();
            await _client.LoginAsync(TokenType.Bot, DcKey);
            await _client.StartAsync();
            await _client.DownloadUsersAsync(_client.Guilds);
            await Task.Delay(-1);
        }


        // Logging
        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }


        // Command Handling
        public async Task RegisterCommands()
        {
            _client.MessageReceived += _client_MessageReceived;

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            SocketUserMessage msg = arg as SocketUserMessage;

            if (!msg.Author.IsBot || msg == null)
            {
                osuCommands oC = new osuCommands();
                SocketCommandContext context = new SocketCommandContext(_client, msg);
                int argPos = 0;

                if (context.Channel.Id == 725364427393597440)
                {
                    await oC.SetOsuRole(context.Message.Content, context);
                }

                await oC.MapAndProfileLinksInChat(msg, context);

                if (msg.HasStringPrefix(Prefix, ref argPos))
                {
                    IResult result = await _commands.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess)
                    {
                        Console.WriteLine(result.Error);
                        Console.WriteLine(result.ErrorReason);
                    }
                    else
                    {
                        Console.WriteLine(msg.Content + " - Used by: " + msg.Author);
                    }

                }
            }
        }

    }
}
