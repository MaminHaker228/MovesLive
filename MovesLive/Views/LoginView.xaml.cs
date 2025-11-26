using System;
using System.Windows;
using System.Windows.Controls;
using MovesLive.ViewModels;

namespace MovesLive.Views
{
    public partial class LoginView : UserControl
    {
        private LoginViewModel _viewModel;

        public event EventHandler LoginSuccessful;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            _viewModel.LoginSuccessful += OnLoginSuccessful;

            // Binding для отображения ошибок
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.ErrorMessage))
                {
                    if (!string.IsNullOrEmpty(_viewModel.ErrorMessage))
                    {
                        ErrorTextBlock.Text = _viewModel.ErrorMessage;
                        ErrorTextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ErrorTextBlock.Visibility = Visibility.Collapsed;
                    }
                }

                if (e.PropertyName == nameof(_viewModel.IsLoading))
                {
                    LoginButton.IsEnabled = !_viewModel.IsLoading;
                    LoginButton.Content = _viewModel.IsLoading ? "Загрузка..." : "Войти";
                }
            };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Username = UsernameTextBox.Text;
            _viewModel.Password = PasswordBox.Password;

            if (_viewModel.LoginCommand.CanExecute(null))
            {
                _viewModel.LoginCommand.Execute(null);
            }
        }

        private void OnLoginSuccessful(object sender, EventArgs e)
        {
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }
    }
}