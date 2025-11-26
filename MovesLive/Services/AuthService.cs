using System;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using MovesLive.Database;
using MovesLive.Models;

namespace MovesLive.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDir, "Data", "moveslive.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public async Task<User> AuthenticateAsync(string login, string password)
        {
            string passwordHash = DatabaseInitializer.HashPassword(password);

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT Id, Login, PasswordHash FROM Users WHERE Login = @login AND PasswordHash = @passwordHash";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                PasswordHash = reader.GetString(2)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> RegisterAsync(string login, string password)
        {
            string passwordHash = DatabaseInitializer.HashPassword(password);

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "INSERT INTO Users (Login, PasswordHash) VALUES (@login, @passwordHash)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                    catch (SQLiteException)
                    {
                        // Пользователь уже существует
                        return false;
                    }
                }
            }
        }
    }
}