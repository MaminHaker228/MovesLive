using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MovesLive.Models;
using MovesLive.Services;
using MovesLive.Views;

namespace MovesLive.ViewModels
{
    public class MoviesViewModel : BaseViewModel
    {
        private readonly MovieService _movieService;
        private ObservableCollection<Movie> _movies;
        private ObservableCollection<string> _categories;
        private string _searchText;
        private string _selectedCategory;
        private bool _isLoading;

        public ObservableCollection<Movie> Movies
        {
            get => _movies;
            set => SetProperty(ref _movies, value);
        }

        public ObservableCollection<string> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = SearchMoviesAsync();
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    _ = FilterByCategoryAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand PlayMovieCommand { get; }
        public ICommand RefreshCommand { get; }

        public MoviesViewModel()
        {
            _movieService = new MovieService();
            Movies = new ObservableCollection<Movie>();
            Categories = new ObservableCollection<string>();

            PlayMovieCommand = new RelayCommand<Movie>(PlayMovie);
            RefreshCommand = new RelayCommand(async _ => await LoadMoviesAsync());

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
            await LoadMoviesAsync();
        }

        private async Task LoadMoviesAsync()
        {
            IsLoading = true;

            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Movies.Clear();
                    foreach (var movie in movies)
                    {
                        Movies.Add(movie);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильмов: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _movieService.GetAllCategoriesAsync();
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Categories.Clear();
                    Categories.Add("Все категории");
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                    SelectedCategory = Categories.FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SearchMoviesAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadMoviesAsync();
                return;
            }

            IsLoading = true;

            try
            {
                var movies = await _movieService.SearchMoviesAsync(SearchText);
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Movies.Clear();
                    foreach (var movie in movies)
                    {
                        Movies.Add(movie);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task FilterByCategoryAsync()
        {
            if (string.IsNullOrEmpty(SelectedCategory) || SelectedCategory == "Все категории")
            {
                await LoadMoviesAsync();
                return;
            }

            IsLoading = true;

            try
            {
                var movies = await _movieService.GetMoviesByCategoryAsync(SelectedCategory);
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Movies.Clear();
                    foreach (var movie in movies)
                    {
                        Movies.Add(movie);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void PlayMovie(Movie movie)
        {
            if (movie == null)
                return;

            var playerWindow = new MoviePlayerView(movie);
            playerWindow.ShowDialog();
        }
    }
}