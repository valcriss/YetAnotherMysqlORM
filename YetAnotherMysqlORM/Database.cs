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

        public static async Task<bool> Update(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int result = await command.ExecuteNonQueryAsync();
                }
            }
            return true;
        }

        public static async Task<bool> Delete(string query)
        {
            bool result = false;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int nbRows = await command.ExecuteNonQueryAsync();
                    result = nbRows > 0;
                }
            }
            return result;
        }

        public static async Task<int?> Insert(string query)
        {
            int? id = null;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int result = await command.ExecuteNonQueryAsync();
                }
                using (MySqlCommand command = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = reader.GetInt32(0);
                        }
                    }
                }
            }
            return id;
        }

        public async static Task<Dictionary<string, string>> QueryOne(string query)
        {
            Dictionary<string, string> record = null;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            record = new Dictionary<string, string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string name = reader.GetName(i);
                                string value = reader.GetValue(i)?.ToString();
                                record.Add(name, value);
                            }
                        }
                    }
                }
            }
            return record;
        }

        public async static Task<List<Dictionary<string, string>>> QueryMultiple(string query)
        {
            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> record = new Dictionary<string, string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string name = reader.GetName(i);
                                string value = reader.GetValue(i)?.ToString();
                                record.Add(name, value);
                            }
                            records.Add(record);
                        }
                    }
                }
            }
            return records;
        }
    }
}
