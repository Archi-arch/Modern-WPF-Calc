using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Calculator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                LogException(args.ExceptionObject as Exception);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                LogException(args.Exception);
                args.Handled = true; // Prevents process crash
            };
        }

        private static void LogException(Exception? ex)
        {
            if (ex == null) return;
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}\n");
                MessageBox.Show($"Calculation or UI error:\n{ex.Message}", "Calculator Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch { }
        }
    }
}
