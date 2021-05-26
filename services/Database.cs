using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ModeratorBot.Services
{
    ///
    public class Database
    {
        private MySqlConnection _database;

        ///
        public Database()
        {
            _database = new MySqlConnection("server=localhost;user=root;database=discord-bot;port=3306;password=root");
        }

        ///
        public int GetBalance(ulong userId)
        {
            try
            {
                _database.Open();
                int balance = 0;
                string sql = $"SELECT points FROM user WHERE user_id = {userId}";
                MySqlCommand cmd = new MySqlCommand(sql, _database);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        balance = reader.GetInt32(0);
                    }
                }
                _database.Close();
                return balance;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        ///
        public async Task UpdateLevel(ulong userId, string username)
        {
            await _database.OpenAsync();
            string sql = $"SELECT * FROM user WHERE user_id={userId}";
            MySqlCommand cmd = new MySqlCommand(sql, _database);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                await _database.CloseAsync();
                InserNewUser(userId, username);
                await UpdateLevel(userId, username);
            }

            if (reader.HasRows)
            {
                await _database.CloseAsync();
                int currentBalance = GetBalance(userId);
                sql = $"UPDATE user SET points={currentBalance + 1} WHERE user_id={userId}";
                await _database.OpenAsync();
                MySqlCommand updateCmd = new MySqlCommand(sql, _database);
                await updateCmd.ExecuteNonQueryAsync();
                await _database.CloseAsync();
            }
        }

        private async void InserNewUser(ulong userId, string username)
        {
            if (_database.State == System.Data.ConnectionState.Closed)
            {
                await _database.OpenAsync();
            }
            string sql = $"INSERT INTO `user` (`user_id`, `nick`, `points`) VALUES ('{userId}', '{username}', '0');";
            MySqlCommand insertCmd = new MySqlCommand(sql, _database);
            await insertCmd.ExecuteNonQueryAsync();
            await _database.CloseAsync();
        }
    }
}