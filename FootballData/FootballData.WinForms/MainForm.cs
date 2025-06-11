using FootballData.DataLayer;
using FootballData.DataLayer.Models;
using System.Globalization;
using System.Resources;
using Point = System.Drawing.Point;

namespace FootballData.WinForms
{
    public partial class MainForm : Form
    {
        private readonly FootballDataService _service;
        private Settings _settings;
        private List<Player> _allPlayers;
        private List<Player> _favoritePlayers;
        private const int MAX_FAVORITE_PLAYERS = 3;
        private int currentLoadedPlayers = 0;
        private const int playersPerLoad = 50;
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;
        private readonly FavoritePlayerService _favoritePlayerService = new FavoritePlayerService();
        //private readonly TeamService _teamService = new TeamService();
        //private readonly MatchService _matchService = new MatchService();
        private readonly PdfExportService _pdfExportService = new PdfExportService();
        //private readonly PlayerStatsService _playerStatsService = new PlayerStatsService();
        //private readonly AttendanceRankingService _attendanceRankingService = new AttendanceRankingService();
        //private readonly PlayerService _playerService = new PlayerService();
        private IFootballDataService _footballDataService;

        public MainForm()
        {
            InitializeComponent();

            _settings = Settings.Load();
            if (_settings.DataSource == "API")
            {
                _footballDataService = new FootballDataService();
            }
            else
            {
                _footballDataService = new FootballDataLocalService(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "FootballData",
                        "WorldCupData"));
            }

            _allPlayers = new List<Player>();
            _favoritePlayers = LoadFavoritePlayersFromFile();
            try
            {
                ApplyLanguage(_settings.Language == "en" ? "en" : "hr");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorApplyingLanguage", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _ = LoadTeams();
            LoadFavoritePlayersUI();
            LoadPlayers();
        }

        private void ApplyLanguage(string languageCode)
        {
            _currentCulture = new CultureInfo(languageCode);
            _resourceManager = new ResourceManager("FootballData.WinForms.Properties.Strings", typeof(MainForm).Assembly);

            btnSettings.Text = _resourceManager.GetString("btnSettings", _currentCulture);
            btnExportToPDF.Text = _resourceManager.GetString("btnExportToPDF", _currentCulture);
            lblSelectTeam.Text = _resourceManager.GetString("lblSelectTeam", _currentCulture);
            btnTeams.Text = _resourceManager.GetString("btnTeams", _currentCulture);
            btnMatches.Text = _resourceManager.GetString("btnMatches", _currentCulture);
            btnRankPlayers.Text = _resourceManager.GetString("btnRankPlayers", _currentCulture);
            btnShowMatchData.Text = _resourceManager.GetString("btnShowMatchData", _currentCulture);
            lblPlayers.Text = _resourceManager.GetString("lblPlayers", _currentCulture);
            lblFavoritePlayers.Text = _resourceManager.GetString("lblFavoritePlayers", _currentCulture);
            btnSaveTeam.Text = _resourceManager.GetString("btnSaveButton", _currentCulture);
            lblTitle.Text = _settings.Championship == "men" ? _resourceManager.GetString("FIFA_WorldCup_Men", _currentCulture) : _resourceManager.GetString("FIFA_WorldCup_Women", _currentCulture);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                _resourceManager.GetString("ExitConfirmationMessage", _currentCulture),
                _resourceManager.GetString("ExitConfirmationTitle", _currentCulture),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void panelAllPlayers_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void panelFavoritePlayers_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private async Task LoadTeams()
        {
            try
            {
                List<Team> teams = await _footballDataService.GetTeamsAsync(_settings.Championship == "men");

                if (teams == null || teams.Count == 0)
                {
                    MessageBox.Show(_resourceManager.GetString("NoAvailableTeams", _currentCulture), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                comboTeams.DataSource = teams.Select(t => $"{t.Country} ({t.Id})").ToList();

                dataGridView.DataSource = teams.Select(t => new
                {
                    t.Country,
                    t.Wins,
                    t.Draws,
                    t.Losses,
                    t.GamesPlayed,
                    GoalDifference = $"{t.GoalsFor} : {t.GoalsAgainst}"
                }).ToList();

                dataGridView.Columns["Country"].HeaderText = _resourceManager.GetString("CountryColumn", _currentCulture);
                dataGridView.Columns["Wins"].HeaderText = _resourceManager.GetString("WinsColumn", _currentCulture);
                dataGridView.Columns["Draws"].HeaderText = _resourceManager.GetString("DrawsColumn", _currentCulture);
                dataGridView.Columns["Losses"].HeaderText = _resourceManager.GetString("LossesColumn", _currentCulture);
                dataGridView.Columns["GamesPlayed"].HeaderText = _resourceManager.GetString("GamesPlayedColumn", _currentCulture);
                dataGridView.Columns["GoalDifference"].HeaderText = _resourceManager.GetString("GoalDifferenceColumn", _currentCulture);

                if (!string.IsNullOrEmpty(_settings.FavoriteTeam))
                {
                    string selectedItem = comboTeams.Items.Cast<string>()
                        .FirstOrDefault(item => item.StartsWith(_settings.FavoriteTeam));

                    if (selectedItem != null)
                    {
                        comboTeams.SelectedItem = selectedItem;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorFetchingTeams", _currentCulture), ex.Message));
            }
        }


        private async Task LoadMatches()
        {
            try
            {
                List<Match> matches = await _footballDataService.GetMatchesAsync(_settings.Championship == "men");

                dataGridView.DataSource = matches.Select(m => new
                {
                    m.Date,
                    m.HomeTeam,
                    HomeGoals = m.HomeTeamScore.Goals,
                    m.AwayTeam,
                    AwayGoals = m.AwayTeamScore.Goals,
                    Winner = m.Winner
                }).ToList();

                dataGridView.Columns["Date"].HeaderText = _resourceManager.GetString("DateColumn", _currentCulture);
                dataGridView.Columns["HomeTeam"].HeaderText = _resourceManager.GetString("HomeTeamColumn", _currentCulture);
                dataGridView.Columns["HomeGoals"].HeaderText = _resourceManager.GetString("HomeGoalsColumn", _currentCulture);
                dataGridView.Columns["AwayTeam"].HeaderText = _resourceManager.GetString("AwayTeamColumn", _currentCulture);
                dataGridView.Columns["AwayGoals"].HeaderText = _resourceManager.GetString("AwayGoalsColumn", _currentCulture);
                dataGridView.Columns["Winner"].HeaderText = _resourceManager.GetString("WinnerColumn", _currentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorFetchingMatches", _currentCulture), ex.Message));
            }
        }

        private async Task LoadPlayers()
        {
            try
            {
                _allPlayers = await _footballDataService.GetPlayersAsync(_settings.Championship, _settings.FavoriteTeam);

                Invoke(new Action(() =>
                {
                    panelAllPlayers.Controls.Clear();
                    currentLoadedPlayers = 0;
                    LoadMorePlayers();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorFetchingPlayers", _currentCulture), ex.Message));
            }
        }

        private void panelAllPlayers_MouseWheel(object sender, MouseEventArgs e)
        {
            if (panelAllPlayers.VerticalScroll.Value + panelAllPlayers.Height >= panelAllPlayers.VerticalScroll.Maximum - 50)
            {
                LoadMorePlayers();
            }
        }

        private void LoadMorePlayers()
        {
            if (currentLoadedPlayers >= _allPlayers.Count)
                return;

            int yOffset = panelAllPlayers.Controls.Count > 0 ? panelAllPlayers.Controls[panelAllPlayers.Controls.Count - 1].Bottom + 5 : 0;
            int endIndex = Math.Min(currentLoadedPlayers + playersPerLoad, _allPlayers.Count);

            for (int i = currentLoadedPlayers; i < endIndex; i++)
            {
                bool isFavorite = _favoritePlayers.Any(p => p.Name == _allPlayers[i].Name);
                var playerControl = new PlayerControl(_allPlayers[i], isFavorite);

                playerControl.MouseDown += PlayerControl_MouseDown;
                playerControl.Location = new Point(5, yOffset);
                panelAllPlayers.Controls.Add(playerControl);
                yOffset += playerControl.Height + 5;
            }
            currentLoadedPlayers = endIndex;
            panelAllPlayers.AutoScroll = true;
        }

        private void SaveFavoritePlayersToFile()
        {
            try
            {
                _favoritePlayerService.SaveFavoritePlayers(_favoritePlayers);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorSavingFavoritePlayers", _currentCulture), ex.Message));
            }
        }

        private List<Player> LoadFavoritePlayersFromFile()
        {
            try
            {
                return _favoritePlayerService.LoadFavoritePlayers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorLoadingFavoritePlayers", _currentCulture), ex.Message));
                return new List<Player>();
            }
        }

        private void LoadFavoritePlayersUI()
        {
            panelFavoritePlayers.Controls.Clear();
            int yOffset = 0;

            foreach (var player in _favoritePlayers)
            {
                var playerControl = new PlayerControl(player, true);
                playerControl.MouseDown += PlayerControl_MouseDown;
                playerControl.Location = new Point(5, yOffset);
                panelFavoritePlayers.Controls.Add(playerControl);
                yOffset += playerControl.Height + 5;
            }
            panelFavoritePlayers.AutoScroll = true;
        }

        private void PlayerControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is PlayerControl playerControl)
            {
                foreach (Control control in panelAllPlayers.Controls)
                {
                    if (control is PlayerControl pc)
                    {
                        pc.BackColor = Color.Transparent;
                    }
                }
                foreach (Control control in panelFavoritePlayers.Controls)
                {
                    if (control is PlayerControl pc)
                    {
                        pc.BackColor = Color.Transparent;
                    }
                }
                playerControl.BackColor = Color.LightBlue;
                DoDragDrop(playerControl, DragDropEffects.Move);
            }
        }

        private void panelAllPlayers_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(PlayerControl)) is PlayerControl playerControl)
            {
                panelAllPlayers.Controls.Add(playerControl);
                _favoritePlayers.Remove(playerControl.Player);
                SaveFavoritePlayersToFile();
                foreach (Control control in panelFavoritePlayers.Controls)
                {
                    if (control is PlayerControl pc && pc.Player.Name == playerControl.Player.Name)
                    {
                        panelFavoritePlayers.Controls.Remove(pc);
                        break;
                    }
                }
                foreach (Control control in panelAllPlayers.Controls)
                {
                    if (control is PlayerControl pc && pc.Player.Name == playerControl.Player.Name)
                    {
                        pc.SetFavorite(false);
                        break;
                    }
                }
            }
        }

        private void panelFavoritePlayers_DragDrop(object sender, DragEventArgs e)
        {
            if (panelFavoritePlayers.Controls.Count >= MAX_FAVORITE_PLAYERS)
            {
                MessageBox.Show(_resourceManager.GetString("MaxFavoritePlayersReached", _currentCulture), "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (e.Data.GetData(typeof(PlayerControl)) is PlayerControl playerControl)
            {
                if (!_favoritePlayers.Any(p => p.Name == playerControl.Player.Name))
                {
                    _favoritePlayers.Add(playerControl.Player);
                    SaveFavoritePlayersToFile();

                    int yOffset = panelFavoritePlayers.Controls.Count > 0 ? panelFavoritePlayers.Controls[panelFavoritePlayers.Controls.Count - 1].Bottom + 5 : 0;

                    var favoritePlayerControl = new PlayerControl(playerControl.Player, true);
                    favoritePlayerControl.MouseDown += PlayerControl_MouseDown;
                    favoritePlayerControl.Location = new Point(5, yOffset);
                    panelFavoritePlayers.Controls.Add(favoritePlayerControl);

                    panelFavoritePlayers.AutoScroll = true;

                    foreach (PlayerControl pc in panelAllPlayers.Controls)
                    {
                        if (pc.Player.Name == playerControl.Player.Name)
                        {
                            pc.SetFavorite(true);
                        }
                    }
                    LoadFavoritePlayersUI();
                }
            }
        }

        private async void btnSaveTeam_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboTeams.SelectedItem != null)
                {
                    string selectedTeam = comboTeams.SelectedItem.ToString().Split('(')[0].Trim();
                    _settings.FavoriteTeam = selectedTeam;

                    _settings.Save();

                    MessageBox.Show(string.Format(_resourceManager.GetString("FavoriteTeamSet", _currentCulture), _settings.FavoriteTeam));

                    _allPlayers.Clear();
                    panelAllPlayers.Controls.Clear();
                    currentLoadedPlayers = 0;

                    await LoadPlayers();
                    await LoadMatches();
                    await LoadTeams();
                }
                else
                {
                    MessageBox.Show(_resourceManager.GetString("SelectTeamError", _currentCulture));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorSavingFavoriteTeam", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnTeams_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadTeams();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorLoadingTeams", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnMatches_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadMatches();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorLoadingMatches", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                SettingsForm settingsForm = new SettingsForm();
                settingsForm.SettingsUpdated += RefreshData;
                settingsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorOpeningSettings", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RefreshData()
        {
            try
            {
                string previousChampionship = _settings.Championship;

                _settings = Settings.Load();

                if (_settings.DataSource == "API")
                {
                    _footballDataService = new FootballDataService();
                }
                else
                {
                    _footballDataService = new FootballDataLocalService(
                        Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "FootballData",
                            "WorldCupData"));
                }

                ApplyLanguage(_settings.Language == "en" ? "en" : "hr");

                if (_settings.Championship != previousChampionship)
                {
                    _favoritePlayers.Clear();
                    SaveFavoritePlayersToFile();

                    panelFavoritePlayers.Controls.Clear();
                    _settings.FavoriteTeam = null;
                }

                _settings.Save();

                await LoadTeams();

                _allPlayers.Clear();
                panelAllPlayers.Controls.Clear();

                await LoadPlayers();

                panelFavoritePlayers.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorRefreshingData", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPlayerStats()
        {
            if (string.IsNullOrEmpty(_settings.FavoriteTeam))
            {
                MessageBox.Show(_resourceManager.GetString("PleaseSelectTeam", _currentCulture), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var playerStats = await _footballDataService.GetPlayerStatsAsync(_settings.Championship, _settings.FavoriteTeam);

                if (playerStats.Count == 0)
                {
                    MessageBox.Show(_resourceManager.GetString("NoPlayerStatsAvailable", _currentCulture), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                dataGridView.DataSource = playerStats
                    .Select(p => new { Player = p.Key, Goals = p.Value.Goals, YellowCards = p.Value.YellowCards, Appearances = p.Value.Appearances })
                    .OrderByDescending(p => p.Goals)
                    .ThenByDescending(p => p.YellowCards)
                    .ThenByDescending(p => p.Appearances)
                    .ToList();

                dataGridView.Columns["Player"].HeaderText = _resourceManager.GetString("PlayerColumn", _currentCulture);
                dataGridView.Columns["Goals"].HeaderText = _resourceManager.GetString("GoalsColumn", _currentCulture);
                dataGridView.Columns["YellowCards"].HeaderText = _resourceManager.GetString("YellowCardsColumn", _currentCulture);
                dataGridView.Columns["Appearances"].HeaderText = _resourceManager.GetString("AppearancesColumn", _currentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorLoadingPlayerStats", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnRankPlayers_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadPlayerStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorRankingPlayers", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShowAttendanceRanking_Click(object sender, EventArgs e)
        {
            LoadAttendanceRanking();
        }

        private async Task LoadAttendanceRanking()
        {
            if (string.IsNullOrEmpty(_settings.FavoriteTeam))
            {
                MessageBox.Show(_resourceManager.GetString("SelectFavoriteTeam", _currentCulture), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var teamMatches = await _footballDataService.GetAttendanceRankingAsync(_settings.Championship, _settings.FavoriteTeam);

                if (teamMatches.Count == 0)
                {
                    MessageBox.Show(_resourceManager.GetString("NoAttendanceData", _currentCulture), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                dataGridView.DataSource = teamMatches
                    .Select(m => new
                    {
                        Location = m.Location,
                        Attendance = m.Attendance.ToString(),
                        Home = m.Home,
                        Away = m.Away
                    })
                    .ToList();

                dataGridView.Columns["Location"].HeaderText = _resourceManager.GetString("LocationColumn", _currentCulture);
                dataGridView.Columns["Attendance"].HeaderText = _resourceManager.GetString("AttendanceColumn", _currentCulture);
                dataGridView.Columns["Home"].HeaderText = _resourceManager.GetString("HomeColumn", _currentCulture);
                dataGridView.Columns["Away"].HeaderText = _resourceManager.GetString("AwayColumn", _currentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(_resourceManager.GetString("ErrorFetchingAttendanceData", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportToPDF_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count == 0 || dataGridView.Columns.Count == 0)
            {
                MessageBox.Show(_resourceManager.GetString("NoDataToExport", _currentCulture), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Save ranking list as PDF";
                saveFileDialog.FileName = "Football Data.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        if (File.Exists(filePath) && IsFileLocked(filePath))
                        {
                            MessageBox.Show(_resourceManager.GetString("PdfFileOpenError", _currentCulture), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var headers = dataGridView.Columns.Cast<DataGridViewColumn>()
                            .Select(c => c.HeaderText).ToArray();

                        var data = dataGridView.Rows.Cast<DataGridViewRow>()
                            .Where(r => !r.IsNewRow)
                            .Select(r => r.Cells.Cast<DataGridViewCell>()
                                .Select(c => c.Value?.ToString()?.Trim() ?? "N/A").ToArray())
                            .ToList();

                        _pdfExportService.ExportToPdf(filePath, data, headers, "Ranking List");

                        MessageBox.Show(_resourceManager.GetString("PdfGenerationSuccess", _currentCulture), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show(string.Format(_resourceManager.GetString("FileAccessError", _currentCulture), ioEx.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(_resourceManager.GetString("UnexpectedError", _currentCulture), ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}