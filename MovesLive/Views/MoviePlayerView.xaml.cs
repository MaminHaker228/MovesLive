using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using MovesLive.Models;
using MovesLive.ViewModels;

namespace MovesLive.Views
{
    public partial class MoviePlayerView : Window
    {
        private MoviePlayerViewModel _viewModel;
        private DispatcherTimer _timer;
        private bool _isDragging = false;

        public MoviePlayerView(Movie movie)
        {
            InitializeComponent();
            
            _viewModel = new MoviePlayerViewModel(movie);
            DataContext = _viewModel;

            // Инициализируем таймер для обновления прогресса
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;

            // Подписываемся на события ViewModel
            _viewModel.PlayRequested += (s, e) => Play();
            _viewModel.PauseRequested += (s, e) => Pause();
            _viewModel.StopRequested += (s, e) => Stop();

            // Устанавливаем источник видео
            if (!string.IsNullOrEmpty(movie.StreamingUrl))
            {
                VideoPlayer.Source = new Uri(movie.StreamingUrl);
            }
            else if (!string.IsNullOrEmpty(movie.LocalSourcePath))
            {
                VideoPlayer.Source = new Uri(movie.LocalSourcePath, UriKind.RelativeOrAbsolute);
            }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan duration = VideoPlayer.NaturalDuration.TimeSpan;
                ProgressSlider.Maximum = duration.TotalSeconds;
                TotalTimeText.Text = FormatTime(duration);
            }
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            Close();
        }

        private void Play()
        {
            VideoPlayer.Play();
            _viewModel.IsPlaying = true;
            PlayPauseButton.Content = "⏸️ Пауза";
            _timer.Start();
        }

        private void Pause()
        {
            VideoPlayer.Pause();
            _viewModel.IsPlaying = false;
            PlayPauseButton.Content = "▶️ Воспроизвести";
            _timer.Stop();
        }

        private void Stop()
        {
            VideoPlayer.Stop();
            _viewModel.IsPlaying = false;
            PlayPauseButton.Content = "▶️ Воспроизвести";
            _timer.Stop();
            ProgressSlider.Value = 0;
            CurrentTimeText.Text = "00:00";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isDragging && VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Value = VideoPlayer.Position.TotalSeconds;
                CurrentTimeText.Text = FormatTime(VideoPlayer.Position);
            }
        }

        private void ProgressSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void ProgressSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            VideoPlayer.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDragging)
            {
                CurrentTimeText.Text = FormatTime(TimeSpan.FromSeconds(e.NewValue));
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer != null)
            {
                VideoPlayer.Volume = e.NewValue;
            }
        }

        private string FormatTime(TimeSpan time)
        {
            if (time.TotalHours >= 1)
                return time.ToString(@"hh\:mm\:ss");
            else
                return time.ToString(@"mm\:ss");
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            VideoPlayer?.Stop();
            base.OnClosed(e);
        }
    }
}