using System.Windows;
using StudentRegistrationApp.ViewModels;

namespace StudentRegistrationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// MVVM pattern: DataContext is set to MainViewModel
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}