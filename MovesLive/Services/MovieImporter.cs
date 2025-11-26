using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MovesLive.Models;

namespace MovesLive.Services
{
    public class MovieImporter
    {
        private readonly MovieService _movieService;
        private readonly string _postersPath;

        public MovieImporter()
        {
            _movieService = new MovieService();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _postersPath = Path.Combine(baseDir, "Assets", "Posters");

            if (!Directory.Exists(_postersPath))
                Directory.CreateDirectory(_postersPath);
        }

        public async Task ImportMoviesAsync()
        {
            // Используем тестовые видео из открытых источников
            var testMovies = new List<Movie>
            {
                new Movie
                {
                    Title = "Big Buck Bunny",
                    Description = "Очаровательный короткометражный фильм о большом добром кролике, который защищает своих друзей.",
                    Category = "Анимация",
                    Year = 2008,
                    Rating = 7.8,
                    Duration = 10,
                    PosterPath = "https://peach.blender.org/wp-content/uploads/title_anouncement.jpg?x11217",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"
                },
                new Movie
                {
                    Title = "Sintel",
                    Description = "Эпическая история о девушке-воине, которая ищет своего потерянного дракона.",
                    Category = "Фэнтези",
                    Year = 2010,
                    Rating = 8.2,
                    Duration = 15,
                    PosterPath = "https://durian.blender.org/wp-content/uploads/2010/06/sintel-poster.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/Sintel.mp4"
                },
                new Movie
                {
                    Title = "Elephant Dream",
                    Description = "Сюрреалистическое путешествие двух друзей через странный механический мир.",
                    Category = "Фэнтези",
                    Year = 2006,
                    Rating = 7.5,
                    Duration = 11,
                    PosterPath = "https://orange.blender.org/wp-content/themes/orange/images/media/posters/16-9-poster.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4"
                },
                new Movie
                {
                    Title = "Tears of Steel",
                    Description = "Научно-фантастический фильм о группе воинов в постапокалиптическом мире.",
                    Category = "Фантастика",
                    Year = 2012,
                    Rating = 8.0,
                    Duration = 12,
                    PosterPath = "https://mango.blender.org/wp-content/uploads/2012/05/01_poster_breakdownreel1.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/TearsOfSteel.mp4"
                },
                new Movie
                {
                    Title = "For Bigger Blazes",
                    Description = "Динамичный ролик с яркими визуальными эффектами.",
                    Category = "Демо",
                    Year = 2015,
                    Rating = 7.2,
                    Duration = 1,
                    PosterPath = "https://storage.googleapis.com/gtv-videos-bucket/sample/images/ForBiggerBlazes.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4"
                },
                new Movie
                {
                    Title = "For Bigger Escape",
                    Description = "Захватывающий визуальный опыт с красивыми пейзажами.",
                    Category = "Демо",
                    Year = 2015,
                    Rating = 7.4,
                    Duration = 1,
                    PosterPath = "https://storage.googleapis.com/gtv-videos-bucket/sample/images/ForBiggerEscapes.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerEscapes.mp4"
                },
                new Movie
                {
                    Title = "For Bigger Fun",
                    Description = "Веселый и яркий ролик с интересными сценами.",
                    Category = "Демо",
                    Year = 2015,
                    Rating = 7.1,
                    Duration = 1,
                    PosterPath = "https://storage.googleapis.com/gtv-videos-bucket/sample/images/ForBiggerFun.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerFun.mp4"
                },
                new Movie
                {
                    Title = "For Bigger Joyrides",
                    Description = "Адреналиновый ролик с быстрыми машинами и динамичными сценами.",
                    Category = "Демо",
                    Year = 2015,
                    Rating = 7.6,
                    Duration = 1,
                    PosterPath = "https://storage.googleapis.com/gtv-videos-bucket/sample/images/ForBiggerJoyrides.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerJoyrides.mp4"
                },
                new Movie
                {
                    Title = "For Bigger Meltdowns",
                    Description = "Впечатляющий ролик с эпическими эффектами.",
                    Category = "Демо",
                    Year = 2015,
                    Rating = 7.3,
                    Duration = 1,
                    PosterPath = "https://storage.googleapis.com/gtv-videos-bucket/sample/images/ForBiggerMeltdowns.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerMeltdowns.mp4"
                },
                new Movie
                {
                    Title = "Subaru Outback On Street And Dirt",
                    Description = "Демонстрация возможностей Subaru Outback на различных типах дорог.",
                    Category = "Авто",
                    Year = 2016,
                    Rating = 7.9,
                    Duration = 2,
                    PosterPath = "https://storage.googleapis.com/gtv-videos-bucket/sample/images/SubaruOutbackOnStreetAndDirt.jpg",
                    StreamingUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/SubaruOutbackOnStreetAndDirt.mp4"
                }
            };

            foreach (var movie in testMovies)
            {
                await _movieService.AddMovieAsync(movie);
            }
        }

        private async Task<string> DownloadPosterAsync(string url, string fileName)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var bytes = await response.Content.ReadAsByteArrayAsync();
                        string filePath = Path.Combine(_postersPath, fileName);
                        await File.WriteAllBytesAsync(filePath, bytes);
                        return filePath;
                    }
                }
            }
            catch
            {
                // Если не удалось скачать, возвращаем оригинальный URL
            }

            return url;
        }
    }
}