using System.Threading.Tasks;
using Discord.Commands;
using ModeratorBot.Services;

namespace ModeratorBot.Modules
{
    ///
    public class Points : ModuleBase<SocketCommandContext>
    {
        private Database _database;

        ///
        public Points(Database database)
        {
            _database = database;
        }

        [Command("balance")]
        private async Task BalanceAsync()
        {
            await ReplyAsync($"Your balance is: {_database.GetBalance(Context.User.Id)}");

        }
    }
}