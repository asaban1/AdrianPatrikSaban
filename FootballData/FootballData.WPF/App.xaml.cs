using System.IO;
using System.Windows;

namespace FootballData.WPF
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FootballData",
                "settings.json"
            );

            Settings settings;

            try
            {
                if (!File.Exists(settingsPath))
                {
                    settings = new Settings();
                    OpenMainWindow(settings);
                    OpenSettingsWindow(settings);
                }
                else
                {
                    settings = Settings.Load();
                    OpenMainWindow(settings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void OpenMainWindow(Settings settings)
        {
            try
            {
                MainWindow mainWindow = new MainWindow(settings);
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening MainWindow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void OpenSettingsWindow(Settings settings)
        {
            try
            {
                SettingsWindow settingsWindow = new SettingsWindow(settings);
                bool? result = settingsWindow.ShowDialog();

                if (result == true)
                {
                    settings = Settings.Load();
                    Application.Current.MainWindow.DataContext = settings;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening SettingsWindow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}
