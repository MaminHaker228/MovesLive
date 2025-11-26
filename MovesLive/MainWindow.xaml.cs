using System.Windows;
using MovesLive.Views;

namespace MovesLive
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Подписываемся на событие успешного входа
            LoginView.LoginSuccessful += OnLoginSuccessful;
        }

        private void OnLoginSuccessful(object sender, System.EventArgs e)
        {
            LoginView.Visibility = Visibility.Collapsed;
            MainView.Visibility = Visibility.Visible;
        }
    }
}