using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MySql.Data.MySqlClient;

namespace ModeratorBot.Services
{
    /// <summary>
    /// Command handling service.
    /// </summary>
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;
        private Database _database;

        /// <summary>
        /// Command handling service.
        /// </summary>
        public CommandHandlingService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            Database database
        )
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _database = database;

            _discord.MessageReceived += MessageReceived;
        }

        /// <summary>
        /// Register Modules.
        /// </summary>
        /// <param name="provider"></param>
        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        /// <summary>
        /// Event that fires when message is received.
        /// </summary>
        /// <param name="rawMessage"></param>
        /// <returns></returns>
        private async Task MessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            //if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;
            if (!message.HasStringPrefix("?", ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if (result.Error.HasValue && result.Error.Value == CommandError.UnknownCommand)
            {
                return;
            }

            if (result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand)
            {
                await context.Channel.SendMessageAsync(result.ToString());
            }
            //await UpdateLevelAsync(context);
        }

        /// <summary>
        /// Event that fires when someone sends a message to add points.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task UpdateLevelAsync(SocketCommandContext context)
        {
            await _database.UpdateLevel(context.User.Id, context.User.Username);
        }
    }
}