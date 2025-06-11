using FootballData.DataLayer;
using FootballData.DataLayer.Models;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FootballData.WPF
{
    public partial class MainWindow : Window
    {
        private Settings settings;
        private readonly IFootballDataService _footballDataService;
        private readonly MatchService _matchService;
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        public MainWindow(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;

            string localDataPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FootballData", "WorldCupData");

            _footballDataService = settings.DataSource.ToUpper() == "API"
                ? new FootballDataService()
                : new FootballDataLocalService(localDataPath);
            _matchService = new MatchService(_footballDataService);
            ApplyWindowSettings();
            InitializeApp();
            UpdateWorldCupTitle();
            ApplyLanguage(settings.Language == "en" ? "en" : "hr");
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;
            txtMatchResult.Visibility = Visibility.Collapsed;
        }

        private void ApplyLanguage(string languageCode)
        {
            _currentCulture = new CultureInfo(languageCode);

            Thread.CurrentThread.CurrentUICulture = _currentCulture;

            if (_resourceManager != null)
                _resourceManager.ReleaseAllResources();

            _resourceManager = new ResourceManager("FootballData.WPF.Properties.Strings2", typeof(MainWindow).Assembly);

            UpdateWorldCupTitle();
            btnSettings.Content = _resourceManager.GetString("Settings", _currentCulture);
            txtFavoriteTeam.Text = _resourceManager.GetString("FavoriteTeam", _currentCulture);
            txtOpponentTeam.Text = _resourceManager.GetString("OpponentTeam", _currentCulture);
            txtMatchInfo.Text = _resourceManager.GetString("MatchInfo", _currentCulture);
        }

        private void OnLanguageChanged(string newLanguage)
        {
            try
            {
                settings.Language = newLanguage;
                ApplyLanguage(newLanguage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorChangingLanguage", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void InitializeApp()
        {
            try
            {
                PopulateFavoriteTeamComboBox();
                SetFavoriteTeamInComboBox();
                ApplyLanguage(settings.Language == "en" ? "en" : "hr");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorInitializingApp", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void DisplayPlayersOnField(List<Player> players, bool isFavoriteTeam)
        {
            if (players.Count == 0)
            {
                MessageBox.Show(
                    _resourceManager.GetString("NoStartingPlayersFound", _currentCulture),
                    "Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            Dictionary<string, double> positionYOffsets = new Dictionary<string, double>
    {
        { "Goalie",  220},
        { "Defender", 80 },
        { "Midfield", 80 },
        { "Forward", 200 }
    };

            double verticalSpacing = 70;

            Dictionary<string, int> positionCounts = new Dictionary<string, int>
    {
        { "Goalie", 0 },
        { "Defender", 0 },
        { "Midfield", 0 },
        { "Forward", 0 }
    };

            foreach (var player in players)
            {
                PlayerControl playerControl = new PlayerControl(player.Name, player.ShirtNumber.ToString(), player.IsCaptain, player.Position);

                double x = 0;
                double y = 0;

                switch (player.Position)
                {
                    case "Goalie":
                        x = 80;
                        y = positionYOffsets["Goalie"];
                        break;
                    case "Defender":
                        x = 150;
                        y = positionYOffsets["Defender"];
                        break;
                    case "Midfield":
                        x = 250;
                        y = positionYOffsets["Midfield"];
                        break;
                    case "Forward":
                        x = 350;
                        y = positionYOffsets["Forward"];
                        break;
                }

                if (!isFavoriteTeam)
                {
                    x = FieldCanvas.ActualWidth - x - 50;
                }

                if (positionCounts[player.Position] > 0)
                {
                    y += positionCounts[player.Position] * verticalSpacing;
                }

                Canvas.SetLeft(playerControl, x);
                Canvas.SetTop(playerControl, y);
                FieldCanvas.Children.Add(playerControl);

                positionCounts[player.Position]++;
            }
        }

        private async void PopulateFavoriteTeamComboBox()
        {
            try
            {
                List<Team> teams = await _footballDataService.GetTeamsAsync(settings.Championship == "men");

                cmbFavoriteTeam.Items.Clear();

                foreach (var team in teams)
                {
                    cmbFavoriteTeam.Items.Add(team);
                }

                SetFavoriteTeamInComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorPopulatingFavoriteTeamComboBox", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SetFavoriteTeamInComboBox()
        {
            try
            {
                if (!string.IsNullOrEmpty(settings.FavoriteTeam))
                {
                    foreach (Team team in cmbFavoriteTeam.Items)
                    {
                        if (team.Country == settings.FavoriteTeam)
                        {
                            cmbFavoriteTeam.SelectedItem = team;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorSettingFavoriteTeamInComboBox", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void cmbFavoriteTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbFavoriteTeam.SelectedItem is Team selectedTeam)
                {
                    RemovePlayersFromField();
                    settings.FavoriteTeam = selectedTeam.Country;
                    LoadTeamsIfBothSelected();
                    ClearPlayerNames();
                    try
                    {
                        settings.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format(_resourceManager.GetString("ErrorSavingFavoriteTeam", _currentCulture), ex.Message),
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                    await PopulateOpponentTeamComboBox(selectedTeam.Country);
                    var opponentTeam = cmbOpponentTeam.SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(opponentTeam))
                    {
                        await DisplayMatchResult(selectedTeam.Country, opponentTeam);
                    }
                    else
                    {
                        txtMatchResult.Visibility = Visibility.Collapsed;
                        txtMatchResult.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorSelectingFavoriteTeam", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RemovePlayersFromField()
        {
            List<UIElement> playersToRemove = new List<UIElement>();

            foreach (UIElement element in FieldCanvas.Children)
            {
                if (element is PlayerControl)
                {
                    playersToRemove.Add(element);
                }
            }

            foreach (var player in playersToRemove)
            {
                FieldCanvas.Children.Remove(player);
            }
        }

        private async void cmbOpponentTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetStats();
            try
            {
                var selectedTeam = cmbFavoriteTeam.SelectedItem as Team;
                var opponentTeam = cmbOpponentTeam.SelectedItem?.ToString();

                if (selectedTeam != null && !string.IsNullOrEmpty(opponentTeam))
                {
                    await DisplayMatchResult(selectedTeam.Country, opponentTeam);
                    LoadTeamsIfBothSelected();
                }
                else
                {
                    txtMatchResult.Visibility = Visibility.Collapsed;
                    txtMatchResult.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorHandlingOpponentTeamSelection", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task<List<Player>> GetStartingPlayersForMatchAsync(string teamName, string opponentName)
        {
            var (startingPlayers, _) = await GetPlayersForMatchAsync(teamName, opponentName);
            return startingPlayers;
        }

        private async void LoadTeamsIfBothSelected()
        {
            if (cmbFavoriteTeam.SelectedItem is Team favoriteTeam && cmbOpponentTeam.SelectedItem is string opponentTeam)
            {
                RemovePlayersFromField();

                List<Player> favoritePlayers = await GetStartingPlayersForMatchAsync(favoriteTeam.Country, opponentTeam);
                List<Player> opponentPlayers = await GetStartingPlayersForMatchAsync(opponentTeam, favoriteTeam.Country);

                if (favoritePlayers.Count == 0 || opponentPlayers.Count == 0)
                {
                    return;
                }

                DisplayPlayersOnField(favoritePlayers, true);
                DisplayPlayersOnField(opponentPlayers, false);
            }
        }

        private async Task<(List<Player> StartingPlayers, List<Player> Substitutes)> GetPlayersForMatchAsync(string teamName, string opponentName)
        {
            try
            {
                var isMen = settings.Championship == "men";
                var matches = await _matchService.GetMatchesAsync(isMen);

                var match = matches.FirstOrDefault(m =>
                    (m.HomeTeam == teamName && m.AwayTeam == opponentName) ||
                    (m.HomeTeam == opponentName && m.AwayTeam == teamName));

                if (match == null)
                {
                    return (new List<Player>(), new List<Player>());
                }

                if (match.HomeTeam == teamName)
                    return (match.HomeTeamStatistics.StartingEleven, match.HomeTeamStatistics.Substitutes);

                return (match.AwayTeamStatistics.StartingEleven, match.AwayTeamStatistics.Substitutes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorFetchingPlayersForMatch", _currentCulture), teamName, opponentName, ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return (new List<Player>(), new List<Player>());
            }
        }

        private async Task PopulateOpponentTeamComboBox(string selectedTeam)
        {
            try
            {
                cmbOpponentTeam.Items.Clear();
                bool isMen = settings.Championship == "men";

                List<string> opponents = await GetOpponentsAsync(selectedTeam, isMen);

                foreach (var opponent in opponents)
                {
                    cmbOpponentTeam.Items.Add(opponent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorPopulatingOpponentTeamComboBox", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public async Task<List<string>> GetOpponentsAsync(string teamName, bool isMen)
        {
            try
            {
                var matches = await _matchService.GetMatchesAsync(isMen);
                var opponents = new HashSet<string>();

                foreach (var match in matches)
                {
                    if (match.HomeTeam == teamName)
                        opponents.Add(match.AwayTeam);
                    else if (match.AwayTeam == teamName)
                        opponents.Add(match.HomeTeam);
                }

                return opponents.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorFetchingOpponentTeams", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return new List<string>();
            }
        }

        private async Task DisplayMatchResult(string selectedTeam, string opponent)
        {
            try
            {
                bool isMen = settings.Championship == "men";
                var matches = await _footballDataService.GetMatchesAsync(isMen);

                var selectedMatch = matches.FirstOrDefault(m =>
                    (m.HomeTeam == selectedTeam && m.AwayTeam == opponent) ||
                    (m.AwayTeam == selectedTeam && m.HomeTeam == opponent));

                string venueLabel = settings.Language == "hr" ? "Stadion" : "Venue";
                string attendanceLabel = settings.Language == "hr" ? "Posjecenost" : "Attendance";
                string notFoundText = settings.Language == "hr" ? "Rezultat utakmice nije pronaden" : "Match result not found";

                if (selectedMatch != null)
                {
                    if (selectedMatch.HomeTeam == selectedTeam)
                    {
                        txtCurrentScore.Text = $"{selectedMatch.HomeTeam} {selectedMatch.HomeTeamScore?.Goals ?? 0} - {selectedMatch.AwayTeamScore?.Goals ?? 0} {selectedMatch.AwayTeam}";
                        DisplayPlayerNames(selectedMatch.HomeTeam, selectedMatch.AwayTeam);
                    }
                    else
                    {
                        txtCurrentScore.Text = $"{selectedMatch.AwayTeam} {selectedMatch.AwayTeamScore?.Goals ?? 0} - {selectedMatch.HomeTeamScore?.Goals ?? 0} {selectedMatch.HomeTeam}";
                        DisplayPlayerNames(selectedMatch.AwayTeam, selectedMatch.HomeTeam);
                    }

                    txtVenue.Text = $"{venueLabel}: {selectedMatch.Venue}";
                    txtAttendance.Text = $"{attendanceLabel}: {selectedMatch.Attendance}";
                    txtMatchResult.Visibility = Visibility.Visible;
                }
                else
                {
                    txtCurrentScore.Text = notFoundText;
                    txtVenue.Text = "";
                    txtAttendance.Text = "";
                    txtMatchResult.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                string errorPrefix = _resourceManager.GetString("ErrorFetchingMatchResult", _currentCulture);
                MessageBox.Show(
                    $"{errorPrefix}: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                txtVenue.Text = "";
                txtAttendance.Text = "";
                txtMatchResult.Visibility = Visibility.Visible;
            }
        }

        private async void DisplayPlayerNames(string teamName, string opponentName)
        {
            try
            {
                var (teamStartingPlayers, teamSubstitutes) = await GetPlayersForMatchAsync(teamName, opponentName);
                var (opponentStartingPlayers, opponentSubstitutes) = await GetPlayersForMatchAsync(opponentName, teamName);

                var matches = await _matchService.GetMatchesAsync(settings.Championship == "men");
                var match = matches.FirstOrDefault(m =>
                    (m.HomeTeam == teamName && m.AwayTeam == opponentName) ||
                    (m.HomeTeam == opponentName && m.AwayTeam == teamName));

                var teamEvents = match?.HomeTeam == teamName ? match.HomeTeamEvents : match.AwayTeamEvents;
                var opponentEvents = match?.HomeTeam == opponentName ? match.HomeTeamEvents : match.AwayTeamEvents;

                txtPlayerNames.Document.Blocks.Clear();

                string startingPlayersLabel = settings.Language == "hr" ? "Pocetni igraci:" : "Starting Players:";
                string substitutesLabel = settings.Language == "hr" ? "Zamjene:" : "Substitutes:";

                AddPlayerSection(teamName, teamStartingPlayers, teamSubstitutes, teamEvents, startingPlayersLabel, substitutesLabel);
                AddPlayerSection(opponentName, opponentStartingPlayers, opponentSubstitutes, opponentEvents, startingPlayersLabel, substitutesLabel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorDisplayingPlayerNames", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddPlayerSection(string teamName, List<Player> startingPlayers, List<Player> substitutes, List<MatchEvent> events, string startingPlayersLabel, string substitutesLabel)
        {
            var paragraph = new Paragraph();

            var teamNameRun = new Run(teamName)
            {
                FontWeight = FontWeights.Bold,
                FontSize = 20
            };
            paragraph.Inlines.Add(teamNameRun);
            txtPlayerNames.Document.Blocks.Add(paragraph);

            AddPlayerList(startingPlayersLabel, startingPlayers, events);
            AddPlayerList(substitutesLabel, substitutes, events);
        }

        private void AddPlayerList(string label, List<Player> players, List<MatchEvent> events)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(label)
            {
                FontWeight = FontWeights.Bold,
                FontSize = 18
            });
            txtPlayerNames.Document.Blocks.Add(paragraph);

            foreach (var player in players)
            {
                var playerParagraph = new Paragraph();
                string playerInfo = $"({player.Position}) #{player.ShirtNumber} {player.Name}";
                if (player.IsCaptain)
                {
                    playerInfo += " (C)";
                }
                playerParagraph.Inlines.Add(new Run(playerInfo));
                txtPlayerNames.Document.Blocks.Add(playerParagraph);

                var playerEvents = events?.Where(e => e.Player == player.Name).ToList();
                if (playerEvents != null && playerEvents.Count > 0)
                {
                    var eventParagraph = new Paragraph();
                    var eventDescriptions = string.Join(" ", playerEvents
                        .GroupBy(e => e.TypeOfEvent)
                        .Select(g => MatchEventSymbolProvider.GetSymbol(g.Key)));

                    eventParagraph.Inlines.Add(new Run(eventDescriptions)
                    {
                        FontSize = 14
                    });

                    txtPlayerNames.Document.Blocks.Add(eventParagraph);
                }
            }
        }

        private void ClearPlayerNames()
        {
            txtPlayerNames.Document.Blocks.Clear();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsWindow settingsWindow = new SettingsWindow(settings);

                settingsWindow.ChampionshipChanged += OnChampionshipChanged;
                settingsWindow.LanguageChanged += OnLanguageChanged;
                settingsWindow.DataSourceChanged += OnDataSourceChanged;

                bool? result = settingsWindow.ShowDialog();

                if (result == true)
                {
                    settings = Settings.Load();
                    ApplyWindowSettings();
                    SetFavoriteTeamInComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorLoadingSettings", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnDataSourceChanged(string newDataSource)
        {
            try
            {
                settings.DataSource = newDataSource;
                settings.FavoriteTeam = null;

                cmbFavoriteTeam.SelectedItem = null;
                cmbOpponentTeam.Items.Clear();
                txtMatchResult.Text = string.Empty;

                UpdateWorldCupTitle();

                PopulateFavoriteTeamComboBox();
                ResetStats();
                ClearPlayerNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorChangingDataSource", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void OnChampionshipChanged(string newChampionship)
        {
            try
            {
                settings.Championship = newChampionship;
                settings.FavoriteTeam = null;

                cmbFavoriteTeam.SelectedItem = null;

                cmbOpponentTeam.Items.Clear();
                txtMatchResult.Text = string.Empty;

                UpdateWorldCupTitle();

                PopulateFavoriteTeamComboBox();
                ResetStats();
                ClearPlayerNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorChangingChampionship", _currentCulture), ex.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void UpdateWorldCupTitle()
        {
            if (_resourceManager == null)
            {
                MessageBox.Show(
                    _resourceManager.GetString("ResourceManagerNotInitialized", _currentCulture),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (settings.Championship == "men")
            {
                txtWorldCupTitle.Text = _resourceManager.GetString("WorldCupTitleMen", _currentCulture);
            }
            else if (settings.Championship == "women")
            {
                txtWorldCupTitle.Text = _resourceManager.GetString("WorldCupTitleWomen", _currentCulture);
            }
            else
            {
                txtWorldCupTitle.Text = string.Empty;
            }
        }

        private void ResetStats()
        {
            txtCurrentScore.Text = "";
            txtVenue.Text = "";
            txtAttendance.Text = "";
        }

        private void ApplyWindowSettings()
        {
            RemovePlayersFromField();
            try
            {
                if (settings.WindowSize == "Fullscreen")
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    string[] resolution = settings.WindowSize.Split('x');
                    if (resolution.Length == 2 &&
                        int.TryParse(resolution[0], out int width) &&
                        int.TryParse(resolution[1], out int height))
                    {
                        Width = width;
                        Height = height;
                    }
                    else
                    {
                        MessageBox.Show(
                            _resourceManager.GetString("InvalidResolutionWarning", _currentCulture),
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        Width = 1280;
                        Height = 720;
                    }
                }
                ResetStats();
                ClearPlayerNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorApplyingWindowSettings", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Width = 1280;
                Height = 720;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(
                    _resourceManager.GetString("ExitConfirmationMessage", _currentCulture),
                    _resourceManager.GetString("ExitConfirmationTitle", _currentCulture),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
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

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void FavoriteTeam_Info(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbFavoriteTeam.SelectedItem is Team selectedTeam)
                {
                    TeamInfoWindow teamInfoWindow = new TeamInfoWindow(
                        selectedTeam.Country,
                        selectedTeam.Id,
                        selectedTeam.GamesPlayed,
                        selectedTeam.Wins,
                        selectedTeam.Losses,
                        selectedTeam.Draws,
                        selectedTeam.GoalsFor,
                        selectedTeam.GoalsAgainst
                    );
                    teamInfoWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show(
                        _resourceManager.GetString("PleaseSelectFavoriteTeam", _currentCulture),
                        _resourceManager.GetString("Warning", _currentCulture),
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorDisplayingFavoriteTeamInfo", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private Team GetCountryFromTeamList(string country)
        {
            try
            {
                var allTeams = cmbFavoriteTeam.Items.Cast<Team>().ToList();

                return allTeams.FirstOrDefault(team => team.Country.Equals(country, StringComparison.OrdinalIgnoreCase));
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorCastingItemsToTeam", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorFindingCountry", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }
        }

        private void OpponentTeam_Info(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbOpponentTeam.SelectedItem != null)
                {
                    string selectedOpponentCountry = cmbOpponentTeam.SelectedItem.ToString();

                    if (!string.IsNullOrEmpty(selectedOpponentCountry))
                    {
                        var opponentTeam = GetCountryFromTeamList(selectedOpponentCountry);

                        if (opponentTeam != null)
                        {
                            TeamInfoWindow teamInfoWindow = new TeamInfoWindow(
                                opponentTeam.Country,
                                opponentTeam.Id,
                                opponentTeam.GamesPlayed,
                                opponentTeam.Wins,
                                opponentTeam.Losses,
                                opponentTeam.Draws,
                                opponentTeam.GoalsFor,
                                opponentTeam.GoalsAgainst
                            );
                            teamInfoWindow.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show(
                                _resourceManager.GetString("NoInformationFoundForOpponentTeam", _currentCulture),
                                _resourceManager.GetString("Error", _currentCulture),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(
                        _resourceManager.GetString("PleaseSelectOpponentTeamFirst", _currentCulture),
                        _resourceManager.GetString("Warning", _currentCulture),
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(_resourceManager.GetString("ErrorDisplayingOpponentTeamInfo", _currentCulture), ex.Message),
                    _resourceManager.GetString("Error", _currentCulture),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
