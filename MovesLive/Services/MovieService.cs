using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using MovesLive.Models;

namespace MovesLive.Services
{
    public class MovieService
    {
        private readonly string _connectionString;

        public MovieService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDir, "Data", "moveslive.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            var movies = new List<Movie>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM Movies ORDER BY Year DESC, Rating DESC";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        movies.Add(MapReaderToMovie(reader));
                    }
                }
            }

            return movies;
        }

        public async Task<List<Movie>> SearchMoviesAsync(string searchText)
        {
            var movies = new List<Movie>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"SELECT * FROM Movies 
                              WHERE Title LIKE @search 
                              OR Description LIKE @search 
                              OR Category LIKE @search
                              ORDER BY Rating DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@search", $"%{searchText}%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            movies.Add(MapReaderToMovie(reader));
                        }
                    }
                }
            }

            return movies;
        }

        public async Task<List<Movie>> GetMoviesByCategoryAsync(string category)
        {
            var movies = new List<Movie>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM Movies WHERE Category = @category ORDER BY Rating DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@category", category);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            movies.Add(MapReaderToMovie(reader));
                        }
                    }
                }
            }

            return movies;
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            var categories = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT DISTINCT Category FROM Movies WHERE Category IS NOT NULL ORDER BY Category";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(reader.GetString(0));
                    }
                }
            }

            return categories;
        }

        public async Task AddMovieAsync(Movie movie)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"INSERT INTO Movies 
                              (Title, Description, Category, Year, Rating, Duration, PosterPath, StreamingUrl, LocalSourcePath)
                              VALUES (@title, @description, @category, @year, @rating, @duration, @posterPath, @streamingUrl, @localSourcePath)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@title", movie.Title);
                    command.Parameters.AddWithValue("@description", movie.Description ?? string.Empty);
                    command.Parameters.AddWithValue("@category", movie.Category ?? string.Empty);
                    command.Parameters.AddWithValue("@year", movie.Year);
                    command.Parameters.AddWithValue("@rating", movie.Rating);
                    command.Parameters.AddWithValue("@duration", movie.Duration);
                    command.Parameters.AddWithValue("@posterPath", movie.PosterPath ?? string.Empty);
                    command.Parameters.AddWithValue("@streamingUrl", movie.StreamingUrl ?? string.Empty);
                    command.Parameters.AddWithValue("@localSourcePath", movie.LocalSourcePath ?? string.Empty);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private Movie MapReaderToMovie(SQLiteDataReader reader)
        {
            return new Movie
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Category = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Year = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                Rating = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
                Duration = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                PosterPath = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                StreamingUrl = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                LocalSourcePath = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
            };
        }
    }
}