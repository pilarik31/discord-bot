using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ModeratorBot.Data;
using MySql.Data.MySqlClient;
using Victoria;

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
        private MySqlConnection _database;

        /// <summary>
        /// Command handling service.
        /// </summary>
        public CommandHandlingService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            MySqlConnection database
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
            //_ = UpdateLevelAsync(context);
        }

        /// <summary>
        /// Event that fires when someone sends a message to add points.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task UpdateLevelAsync(SocketCommandContext context)
        {
            _database.Open();
            string sql = $"SELECT * FROM user WHERE user_id={context.User.Id}";
            MySqlCommand cmd = new MySqlCommand(sql, _database);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    _database.Close();
                }

            }
            else
            {
                _database.Close();
                _database.Open();
                sql = $"INSERT INTO `user` (`user_id`, `nick`, `points`) VALUES ('{context.User.Id}', '{context.User.Username}', '0');";
                MySqlCommand insertCmd = new MySqlCommand(sql, _database);
                insertCmd.ExecuteNonQuery();
                _database.Close();

            }
            _database.Clone();

            return Task.CompletedTask;
        }
    }
}