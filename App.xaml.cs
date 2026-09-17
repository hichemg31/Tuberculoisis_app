using System.Windows;

namespace testWPF;

/// <summary>
/// Application entry point.
/// 
/// When DevExpress is installed, add theme initialization here:
///     DevExpress.Xpf.Core.ApplicationThemeHelper.ApplicationThemeName = 
///         DevExpress.Xpf.Core.Theme.Office2019BlackName;
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Catch any unhandled exceptions to show a message box instead of silently dying
        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show(
                $"Erreur inattendue:\n\n{args.Exception.Message}\n\n{args.Exception.InnerException?.Message}\n\nStack:\n{args.Exception.StackTrace}",
                "Erreur",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            args.Handled = true;
        };

        base.OnStartup(e);
    }
}
