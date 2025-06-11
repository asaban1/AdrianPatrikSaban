using Newtonsoft.Json;
using System.Globalization;
using System.Resources;

namespace FootballData.WinForms
{
    public partial class SettingsForm : Form
    {
        private Settings _settings;
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        public event Action SettingsUpdated;

        public SettingsForm()
        {
            InitializeComponent();
            _settings = Settings.Load();

            try
            {
                ApplyLanguage(_settings.Language == "en" ? "en" : "hr");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorApplyingLanguage", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoadSettings();

            KeyPreview = true;
            KeyDown += new KeyEventHandler(SettingsForm_KeyDown);
        }

        private void ApplyLanguage(string languageCode)
        {
            _currentCulture = new CultureInfo(languageCode);
            _resourceManager = new ResourceManager("FootballData.WinForms.Properties.Strings", typeof(MainForm).Assembly);

            lblChampionship.Text = _resourceManager.GetString("lblChampionship", _currentCulture);
            lblLanguage.Text = _resourceManager.GetString("lblLanguage", _currentCulture);
            lblData.Text = _resourceManager.GetString("lblData", _currentCulture);
            btnSave.Text = _resourceManager.GetString("btnSave", _currentCulture);
            lblData.Text = _resourceManager.GetString("lblData", _currentCulture);
        }

        private void ApplySelectedLanguage()
        {
            if (comboLanguage.SelectedItem != null)
            {
                var selectedLanguage = comboLanguage.SelectedItem.ToString();
                _settings.Language = selectedLanguage;
            }
        }

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveSettings();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CancelSettings();
            }
        }

        private void SaveSettings()
        {
            if (comboChampionship.SelectedItem == null || comboLanguage.SelectedItem == null || comboDataSource.SelectedItem == null)
            {
                MessageBox.Show(_resourceManager.GetString("SelectAllFields", _currentCulture));
                return;
            }

            _settings.Championship = comboChampionship.SelectedItem.ToString();
            ApplySelectedLanguage();

            _settings.DataSource = comboDataSource.SelectedItem.ToString();

            try
            {
                var oldSettings = Settings.Load();
                bool isChampionshipChanged = oldSettings.Championship != _settings.Championship;

                if (isChampionshipChanged)
                {
                    _settings.FavoritePlayers.Clear();
                    File.WriteAllText("favorite_players.txt", "[]");
                    File.WriteAllText("all_players.txt", "[]");
                }

                _settings.Save();
                MessageBox.Show(_resourceManager.GetString("SettingsSaved", _currentCulture));
                SettingsUpdated?.Invoke();

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show(_resourceManager.GetString("ErrorDeserializingSettings", _currentCulture) + $"\n{jsonEx.Message}");
            }
            catch (IOException ioEx)
            {
                MessageBox.Show(_resourceManager.GetString("ErrorReadingFile", _currentCulture) + $"\n{ioEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(_resourceManager.GetString("UnexpectedError", _currentCulture) + $"\n{ex.Message}");
            }
        }

        private void LoadSettings()
        {
            try
            {
                _settings = Settings.Load();
                comboChampionship.SelectedItem = _settings.Championship;
                comboLanguage.SelectedItem = _settings.Language == "en" ? "en" : "hr";
                comboDataSource.SelectedItem = _settings.DataSource ?? "API";
            }
            catch (Exception ex)
            {
                MessageBox.Show(_resourceManager.GetString("UnexpectedError", _currentCulture) + $"\n{ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void CancelSettings()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
