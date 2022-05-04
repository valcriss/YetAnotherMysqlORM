using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherMysqlORM
{
    public static class Database
    {
        private static string ConnectionString = "";

        public static void Initialize(string hostname, string database, string username, string password)
        {
            ConnectionString = $"Server={hostname};Database={database};Uid={username};Pwd={password};";
        }

        public static async Task<bool> Execute(string query)
        {
            using(MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int result = await command.ExecuteNonQueryAsync();
                }
            }
            return true;
        }
    }
}
