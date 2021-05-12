using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;

namespace ModeratorBot.Modules
{
    /// <summary>
    /// Administrator command module.
    /// </summary>
    public class Administrator : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Storage instance
        /// </summary>
        private Storage _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        public Administrator(Storage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Debug command.
        /// </summary>
        /// <returns></returns>
        [Command("debug")]
        [Summary("Debugging command.")]
        [Alias("dbg", "db", "d")]
        [RequireOwner]
        public async Task DebugAsync()
        {
            await Context.Channel.SendFileAsync("sounds/ok.mp3");
        }

        /// <summary>
        /// Shut down command.
        /// </summary>
        /// <returns></returns>
        [Command("shutdown")]
        [Summary("Shutdowns bot.")]
        [Alias("sd")]
        [RequireOwner]
        public async Task ShutdownAsync()
        {
            await Context.Channel.SendMessageAsync(_storage.GetKeyValue("shutdown_result"));
            Environment.Exit(0);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Removes X messages from the current channel.
        /// </summary>
        [Command("purge")]
        [Summary("Removes __amount__ messages from the current channel.")]
        //[RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        [RequireBotPermission(Discord.ChannelPermission.ManageMessages)]
        public async Task purgeAsync(
            [Summary("Number of messages to purge.")]int amount
        )
        {
            if (amount <= 0)
            {
                await ReplyAsync(_storage.GetKeyValue("purge_notpositive"));
                return;
            }

            var messages = await this.Context.Channel.GetMessagesAsync(
                Context.Message,
                Discord.Direction.Before,
                limit: amount
            ).FlattenAsync();

            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
            var count = filteredMessages.Count();

            if(count == 0)
            {
                await ReplyAsync(_storage.GetKeyValue("purge_emptydelete"));
                return;
            }

            await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
            await ReplyAsync(
                String.Format(
                    _storage.GetKeyValue("purge_result"),
                    count,
                    (count > 1 ? _storage.GetKeyValue("messages_plural") : _storage.GetKeyValue("messages_singular"))
                )
            );
        }
    }
}