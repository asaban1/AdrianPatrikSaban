using FootballData.DataLayer.Models;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FootballData.WPF
{
    public partial class PlayerStats : Window
    {
        private Player _player;
        private List<MatchEvent> _matchEvents;

        public PlayerStats(Player player, List<MatchEvent> matchEvents)
        {
            InitializeComponent();
            _player = player;
            _matchEvents = matchEvents;
            this.Owner = Application.Current.MainWindow;
            LoadPlayerStats();
        }

        private void LoadPlayerImage(string playerName)
        {
            string imageFolder = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                "FootballData",
                "Players"
            );
            string imagePath = Path.Combine(imageFolder, $"{playerName.Replace(" ", "_")}.jpg");

            if (File.Exists(imagePath))
            {
                try
                {
                    BitmapImage playerImage = new BitmapImage(new System.Uri(imagePath));
                    PlayerImage.Source = playerImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        private void LoadPlayerStats()
        {
            PlayerNameTextBlock.Text = _player.Name;
            if (_player.IsCaptain)
            {
                PlayerNameTextBlock.Text += " (C)";
            }
            PlayerNumberTextBlock.Text = $"#{_player.ShirtNumber}";
            PlayerPositionTextBlock.Text = _player.Position;

            var regularGoals = _matchEvents.Count(e => e.Player == _player.Name && e.TypeOfEvent == "goal");
            var penaltyGoals = _matchEvents.Count(e => e.Player == _player.Name && e.TypeOfEvent == "goal-penalty");
            var totalGoals = regularGoals + penaltyGoals;

            PlayerGoalsTextBlock.Text = penaltyGoals > 0
                ? $"Goals: {totalGoals} ({penaltyGoals} penalties)"
                : $"Goals: {totalGoals}";

            var yellowCards = _matchEvents.Count(e => e.Player == _player.Name && e.TypeOfEvent == "yellow-card");

            PlayerYellowCardsTextBlock.Text = $"Yellow Cards: {yellowCards}";

            LoadPlayerImage(_player.Name);
        }
    }
}
