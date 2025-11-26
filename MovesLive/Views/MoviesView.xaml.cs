using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MovesLive.Models;
using MovesLive.ViewModels;

namespace MovesLive.Views
{
    public partial class MoviesView : UserControl
    {
        private MoviesViewModel _viewModel;

        public MoviesView()
        {
            InitializeComponent();
            _viewModel = new MoviesViewModel();
            DataContext = _viewModel;
        }

        private void MovieCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Movie movie)
            {
                _viewModel.PlayMovieCommand.Execute(movie);
            }
        }
    }
}