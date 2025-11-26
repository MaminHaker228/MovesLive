using System.Windows.Controls;
using MovesLive.ViewModels;

namespace MovesLive.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}