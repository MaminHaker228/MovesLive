using System;
using System.Windows.Input;
using MovesLive.Models;

namespace MovesLive.ViewModels
{
    public class MoviePlayerViewModel : BaseViewModel
    {
        private Movie _movie;
        private bool _isPlaying;
        private double _volume = 0.5;

        public Movie Movie
        {
            get => _movie;
            set => SetProperty(ref _movie, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        public double Volume
        {
            get => _volume;
            set => SetProperty(ref _volume, value);
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand StopCommand { get; }

        public event EventHandler PlayRequested;
        public event EventHandler PauseRequested;
        public event EventHandler StopRequested;

        public MoviePlayerViewModel(Movie movie)
        {
            Movie = movie;
            PlayPauseCommand = new RelayCommand(_ => PlayPause());
            StopCommand = new RelayCommand(_ => Stop());
        }

        private void PlayPause()
        {
            if (IsPlaying)
            {
                PauseRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                PlayRequested?.Invoke(this, EventArgs.Empty);
            }

            IsPlaying = !IsPlaying;
        }

        private void Stop()
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
            IsPlaying = false;
        }
    }
}