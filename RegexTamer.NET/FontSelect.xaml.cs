using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using RegexTamer.NET.ViewModels;

namespace RegexTamer.NET
{
    /// <summary>
    /// FontSelect.xaml の相互作用ロジック
    /// </summary>
    public partial class FontSelect : Window
    {
        public FontSelect()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<IFontSelectViewModel>();
            var _Messege = Ioc.Default.GetService<IMessenger>() ?? throw new NullReferenceException(nameof(IMessenger));
            _Messege.Register<CloseFontSelect>(this, (_, _) => this.Close());
        }

        /// <summary>
        /// Use Ctrl + mouse wheel to zoom in and out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
            {
                var _Settings = Ioc.Default.GetService<ISettings>() ?? throw new NullReferenceException(nameof(ISettings));
                if (e.Delta > 0) { _Settings.FontSizeLarge(); }
                else { _Settings.FontSizeSmall(); }
                e.Handled = true;
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }
    }
}
