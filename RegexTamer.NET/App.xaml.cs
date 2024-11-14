﻿using System.Runtime.InteropServices;
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
        /// 古い DPI Aware
        /// </summary>
        /// <returns></returns>
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetProcessDPIAware();

        /// <summary>
        /// 新しいDPI Aware
        /// </summary>
        /// <param name="awareness"></param>
        /// <returns></returns>
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetProcessDpiAwareness(ProcessDpiAwareness awareness);

        /// <summary>
        /// 新しい DPI Aware で使う引数
        /// </summary>
        private enum ProcessDpiAwareness
        {
            ProcessDpiUnaware = 0,
            ProcessSystemDpiAware = 1,
            ProcessPerMonitorDpiAware = 2
        }

        /// <summary>
        /// サービスの登録
        /// </summary>
        public App()
        {
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
        }

        /// <summary>
        /// 機動処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // アプリケーションをDPI Awareに設定
            if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 3)
            {
                // 新しい SetProcessDpiAwareness
                SetProcessDpiAwareness(ProcessDpiAwareness.ProcessPerMonitorDpiAware);
            }
            else
            {
                // Windows 8.1以前の場合は、SetProcessDpiAwareを使用する（非推奨）
                SetProcessDPIAware();
            }

            base.OnStartup(e);
        }

        /// <summary>
        /// 現在の App インスタンスを使うために取得する
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// サービスプロバイダ
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// サービスをここで登録する
        /// </summary>
        /// <returns></returns>
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMessenger, WeakReferenceMessenger>();
            services.AddSingleton<ISettings, Settings>();
            // ViewModel
            services.AddSingleton<IMainWinodowViewModel, MainWinodowViewModel>();
            services.AddSingleton<IFontSelectViewModel, FontSelectViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
