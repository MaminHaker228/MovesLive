using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MovesLive.Database
{
    public class DatabaseInitializer
    {
        private readonly string _databasePath;
        private readonly string _connectionString;

        public DatabaseInitializer()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "Data");
            
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            _databasePath = Path.Combine(dataDir, "moveslive.db");
            _connectionString = $"Data Source={_databasePath};Version=3;";
        }

        public async Task InitializeAsync()
        {
            bool isNewDatabase = !File.Exists(_databasePath);

            // Создаем файл базы данных
            SQLiteConnection.CreateFile(_databasePath);

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Создаем таблицы
                await CreateTablesAsync(connection);

                // Если база новая, добавляем тестовые данные
                if (isNewDatabase)
                {
                    await InsertTestUserAsync(connection);
                }
            }
        }

        private async Task CreateTablesAsync(SQLiteConnection connection)
        {
            string createUserTableSql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT UNIQUE NOT NULL,
                    PasswordHash TEXT NOT NULL
                );";

            string createMoviesTableSql = @"
                CREATE TABLE IF NOT EXISTS Movies (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Category TEXT,
                    Year INTEGER,
                    Rating REAL,
                    Duration INTEGER,
                    PosterPath TEXT,
                    StreamingUrl TEXT,
                    LocalSourcePath TEXT
                );";

            string createIndexesSql = @"
                CREATE INDEX IF NOT EXISTS idx_movies_title ON Movies(Title);
                CREATE INDEX IF NOT EXISTS idx_movies_category ON Movies(Category);
                CREATE INDEX IF NOT EXISTS idx_movies_year ON Movies(Year);";

            using (var command = new SQLiteCommand(createUserTableSql, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            using (var command = new SQLiteCommand(createMoviesTableSql, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            using (var command = new SQLiteCommand(createIndexesSql, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertTestUserAsync(SQLiteConnection connection)
        {
            string passwordHash = HashPassword("123456");
            
            string insertUserSql = @"
                INSERT INTO Users (Login, PasswordHash) 
                VALUES (@login, @passwordHash);";

            using (var command = new SQLiteCommand(insertUserSql, connection))
            {
                command.Parameters.AddWithValue("@login", "admin");
                command.Parameters.AddWithValue("@passwordHash", passwordHash);
                await command.ExecuteNonQueryAsync();
            }
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}