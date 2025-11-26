using System;
using System.IO;
using System.Windows;
using MovesLive.Database;
using MovesLive.Services;

namespace MovesLive
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Создаем папки, если их нет
                string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                string postersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Posters");

                if (!Directory.Exists(dataPath))
                    Directory.CreateDirectory(dataPath);

                if (!Directory.Exists(postersPath))
                    Directory.CreateDirectory(postersPath);

                // Инициализация базы данных
                var dbInitializer = new DatabaseInitializer();
                await dbInitializer.InitializeAsync();

                // Проверяем, нужно ли импортировать фильмы
                var movieService = new MovieService();
                var movies = await movieService.GetAllMoviesAsync();

                if (movies.Count == 0)
                {
                    // Импортируем тестовые фильмы при первом запуске
                    var importer = new MovieImporter();
                    await importer.ImportMoviesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации приложения: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}