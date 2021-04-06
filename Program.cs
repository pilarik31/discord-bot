using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using ModeratorBot.Services;
using LiteDB;
using MySql.Data.MySqlClient;

namespace ModeratorBot
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public class ModeratorBot
    {
        private DiscordSocketClient _client;
        //private CommandService _commands;
        //private CommandHandler _handler;
        //private Storage _storage;
        private IConfiguration _config;

        
        /// <summary>
        /// Main method made async.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
            => new ModeratorBot().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Main entry method.
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();
            
            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();
            

            await _client.SetGameAsync("you", null, ActivityType.Watching);
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<AudioService>()
                .AddLogging()
                .AddSingleton<LogService>()
                .AddSingleton(_config)
                .AddSingleton<Storage>()
                .AddSingleton(new LiteDatabase("bot.db"))
                .AddSingleton(new MySqlConnection("server=localhost;user=admin;database=discord-bot;port=3306;password=admin"))
                .BuildServiceProvider();
        }
        
        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }
    }
}
