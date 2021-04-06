using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace ModeratorBot.Modules
{
    /// <summary>
    /// Basic commands module.
    /// </summary>
    public class Basic : ModuleBase<SocketCommandContext>
    {

        private Storage _storage;
        private CommandService _commandService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Basic(Storage storage, CommandService commandService)
        {
            _storage = storage;
            _commandService = commandService;
        }

        /// <summary>
        /// Hello world!
        /// </summary>
        /// <returns></returns>
        [Command("hello")]
        [Summary("Says hello.")]
        public async Task HelloWorldAsync()
        {
            await ReplyAsync(_storage.GetKeyValue("helloworld_result"));
        }

        /// <summary>
        /// Generates an embed with specified user info.
        /// </summary>
        /// <param name="user">Desired user.</param>
        /// <returns></returns>
        [Command("info")]
        [Summary("Bot info.")]
        public async Task InfoAsync(IUser user)
        {
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = user.ToString(),
                    IconUrl = user.GetAvatarUrl()
                },
                Color = new Color(255, 0, 0),
                Timestamp = DateTime.Now,
                Title = "User profile",
            };
            embed.AddField("Info", "Account created at: " + user.CreatedAt);

            await ReplyAsync("", embed: embed.Build());
        }

        /// <summary>
        /// Rolls a dice.
        /// </summary>
        /// <returns></returns>
        [Command("roll")]
        [Summary("Rolls a dice.")]
        public async Task RollDiceAsync()
        {
            string user = Context.User.Username;
            Random rnd = new Random();
            int roll = rnd.Next(1, 101);
            string msg = _storage.GetKeyValue("roll_result");
            await ReplyAsync(String.Format(msg, user, roll));
        }

        ///
        [Command("help")]
        [Summary("This.")]
        public async Task Help()
        {
            List<ModuleInfo> modules = _commandService.Modules.ToList();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Help");

            foreach (ModuleInfo module in modules)
            {
                string text = "";
                foreach (var command in module.Commands)
                {
                    text += "`" + command.Name + "`\n";
                    text += command.Summary + "\n";

                    if (command.Parameters.Count > 0)
                    {
                        text += "*Parameters:*\n";
                        foreach (var param in command.Parameters)
                        {
                            text += "__" + param.Name + "__: " + param.Summary + "\n";
                        }
                    }
                }
                embed.AddField(module.Name, text);
            }
            await ReplyAsync(embed: embed.Build());
        }
    }
}