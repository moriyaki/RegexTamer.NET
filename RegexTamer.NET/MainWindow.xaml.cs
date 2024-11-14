using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using RegexTamer.NET.ViewModels;

namespace RegexTamer.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<IMainWinodowViewModel>();
        }

        private void TextBoxSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var _MainWindowViewModel = Ioc.Default.GetService<IMainWinodowViewModel>() ?? throw new NullReferenceException(nameof(IMainWinodowViewModel));
                if (_MainWindowViewModel.RegexErrorStatus == RegexErrorStatus.None)
                {
                    _MainWindowViewModel.ButtonSearchCommand.Execute(null);
                }
            }
        }
    }
}