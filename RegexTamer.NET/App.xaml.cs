using System.Runtime.InteropServices;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using RegexTamer.NET.ViewModels;

namespace RegexTamer.NET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Old DPI Aware
        /// </summary>
        /// <returns></returns>
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetProcessDPIAware();

        /// <summary>
        /// New DPI Aware
        /// </summary>
        /// <param name="awareness"></param>
        /// <returns></returns>
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetProcessDpiAwareness(ProcessDpiAwareness awareness);

        /// <summary>
        /// Arguments used in the new DPI Aware
        /// </summary>
        private enum ProcessDpiAwareness
        {
            ProcessDpiUnaware = 0,
            ProcessSystemDpiAware = 1,
            ProcessPerMonitorDpiAware = 2
        }

        /// <summary>
        /// Register for Services
        /// </summary>
        public App()
        {
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
        }

        /// <summary>
        /// Startup process
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 3)
            {
                SetProcessDpiAwareness(ProcessDpiAwareness.ProcessPerMonitorDpiAware);
            }
            else
            {
                SetProcessDPIAware();
            }

            base.OnStartup(e);
        }

        /// <summary>
        /// Get to use current App instance
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Service Provider
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Service Registeration
        /// </summary>
        /// <returns></returns>
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMessenger, WeakReferenceMessenger>();
            services.AddSingleton<ISettings, Settings>();

            services.AddSingleton<IMainWinodowViewModel, MainWinodowViewModel>();
            services.AddSingleton<IFontSelectViewModel, FontSelectViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
