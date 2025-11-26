using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MovesLive.Services;

namespace MovesLive.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isLoading;

        public event EventHandler<EventArgs> LoginSuccessful;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin());
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Password) && 
                   !IsLoading;
        }

        private async Task LoginAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.AuthenticateAsync(Username, Password);

                if (user != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoginSuccessful?.Invoke(this, EventArgs.Empty);
                    });
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}