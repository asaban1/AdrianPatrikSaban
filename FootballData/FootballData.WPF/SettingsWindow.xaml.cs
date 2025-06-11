using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows;
using System.Windows.Input;

namespace FootballData.WPF
{
    public partial class SettingsWindow : Window
    {
        private Settings appSettings;
        public event Action<string> ChampionshipChanged;
        public event Action<string> LanguageChanged;
        public event Action<string> DataSourceChanged;
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;


        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            appSettings = settings ?? new Settings();
            _resourceManager = new ResourceManager("FootballData.WPF.Properties.Strings2", typeof(SettingsWindow).Assembly);
            ApplyLanguage(Thread.CurrentThread.CurrentUICulture.Name);
            InitializeSettingsUI();
            this.KeyDown += SettingsWindow_KeyDown;
        }

        private void ApplyLanguage(string languageCode)
        {
            _currentCulture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = _currentCulture;

            txtTournament.Text = _resourceManager.GetString("Tournament", _currentCulture);
            txtLanguage.Text = _resourceManager.GetString("Language", _currentCulture);
            txtWindowSize.Text = _resourceManager.GetString("WindowSize", _currentCulture);
            txtDataSource.Text = _resourceManager.GetString("DataSource", _currentCulture);
            btnSave.Content = _resourceManager.GetString("Save", _currentCulture);
            btnCancel.Content = _resourceManager.GetString("Cancel", _currentCulture);
            btnOpenWinFormsApp.Content = _resourceManager.GetString("OpenWinFormsApp", _currentCulture);
        }

        private void OpenWinFormsApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string winFormsAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FootballData.WinForms.exe");

                Process.Start(winFormsAppPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorOpeningWinFormsApp", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void InitializeSettingsUI()
        {
            try
            {
                cmbChampionship.Text = appSettings.Championship ?? "";
                cmbLanguage.Text = appSettings.Language ?? "";
                cmbWindowSize.Text = string.IsNullOrEmpty(appSettings.WindowSize) ? "1280x720" : appSettings.WindowSize;
                cmbDataSource.Text = string.IsNullOrEmpty(appSettings.DataSource) ? "API" : appSettings.DataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorInitializingSettingsUI", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FootballData",
                    "settings.json"
                );

                if (!File.Exists(settingsPath))
                {
                    MessageBox.Show("Settings are not configured. The application will close.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown();
                }
                else
                {
                    e.Cancel = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorDuringWindowClosing", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                appSettings.Championship = cmbChampionship.Text;
                ChampionshipChanged?.Invoke(appSettings.Championship);

                appSettings.Language = cmbLanguage.Text;
                LanguageChanged?.Invoke(appSettings.Language);

                appSettings.WindowSize = cmbWindowSize.Text;

                appSettings.DataSource = cmbDataSource.Text;
                DataSourceChanged?.Invoke(appSettings.DataSource);

                appSettings.Save();
                MessageBox.Show(
                    _resourceManager.GetString("SettingsSavedSuccessfully", _currentCulture),
                    _resourceManager.GetString("Success", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FootballData",
                    "settings.json"
                );

                if (!File.Exists(settingsPath))
                {
                    appSettings.Championship = "men";
                    appSettings.Language = "en";
                    appSettings.WindowSize = "1280x720";

                    string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FootballData");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    appSettings.Save();
                    MessageBox.Show(
                        _resourceManager.GetString("SettingsNotConfiguredDefaultApplied", _currentCulture),
                        _resourceManager.GetString("Information", _currentCulture),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    if (string.IsNullOrEmpty(appSettings.Championship) || string.IsNullOrEmpty(appSettings.Language) || string.IsNullOrEmpty(appSettings.WindowSize))
                    {
                        appSettings.Championship = "men";
                        appSettings.Language = "en";
                        appSettings.WindowSize = "1280x720";

                        string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FootballData");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        appSettings.Save();
                        MessageBox.Show(
                            _resourceManager.GetString("SettingsNotConfiguredDefaultApplied", _currentCulture),
                            _resourceManager.GetString("Information", _currentCulture),
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorDuringSettingsCancel", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SettingsWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    SaveSettings_Click(sender, e);
                }
                else if (e.Key == Key.Escape)
                {
                    CancelSettings_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorHandlingKeyEvent", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
