using System.Windows.Input;

namespace MovesLive.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand ShowMoviesCommand { get; }

        public MainViewModel()
        {
            ShowMoviesCommand = new RelayCommand(_ => ShowMovies());
            
            // По умолчанию показываем фильмы
            ShowMovies();
        }

        private void ShowMovies()
        {
            CurrentView = new MoviesViewModel();
        }
    }
}