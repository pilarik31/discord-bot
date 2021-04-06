using System.Threading.Tasks;
using Discord.Commands;
using MySql.Data.MySqlClient;

namespace ModeratorBot.Modules
{
    ///
    public class Points : ModuleBase<SocketCommandContext>
    {
        private MySqlConnection _database;

        ///
        public Points(MySqlConnection database)
        {
            _database = database;
        }

        [Command("balance")]
        private async Task BalanceAsync()
        {
            try
            {
                _database.Open();
                string sql = $"SELECT points FROM user WHERE user_id =  {Context.User.Id}";
                MySqlCommand cmd = new MySqlCommand(sql, _database);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        await ReplyAsync($"Your balance is: {reader.GetInt32(0).ToString()}");
                    }
                }
                _database.Close();
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}