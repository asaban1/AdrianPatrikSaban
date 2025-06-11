using FootballData.DataLayer;
using FootballData.DataLayer.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace FootballData.WPF
{
    public partial class PlayerControl : UserControl
    {
        private bool _isCaptain;
        private string _position;
        private readonly FootballDataService _footballDataService;
        private readonly PlayerStatsService _playerStatsService;

        public PlayerControl(string playerName, string shirtNumber, bool isCaptain, string position)
        {
            InitializeComponent();
            PlayerName.Text = playerName;
            PlayerNumber.Text = $"#{shirtNumber}";
            _isCaptain = isCaptain;
            _position = position;

            _footballDataService = new FootballDataService();
            _playerStatsService = new PlayerStatsService();

            PlayerBorderBrush.Color = _isCaptain ? Colors.Goldenrod : Colors.Black;

            LoadPlayerImage(playerName);
        }

        private void LoadPlayerImage(string playerName)
        {
            string imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FootballData", "Players");
            string imagePath = Path.Combine(imageFolder, $"{playerName.Replace(" ", "_")}.jpg");

            if (File.Exists(imagePath))
            {
                try
                {
                    BitmapImage playerImage = new BitmapImage(new Uri(imagePath));
                    PlayerImageBrush.ImageSource = playerImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }

        private async void PlayerButton_Click(object sender, RoutedEventArgs e)
        {
            Player player = new Player()
            {
                Name = PlayerName.Text,
                ShirtNumber = int.Parse(PlayerNumber.Text.Trim('#')),
                IsCaptain = _isCaptain,
                Position = _position
            };

            List<MatchEvent> matchEvents = await _playerStatsService.GetMatchEventsForPlayer(player);

            PlayerStats playerStatsWindow = new PlayerStats(player, matchEvents);
            playerStatsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            playerStatsWindow.Left = SystemParameters.PrimaryScreenWidth;
            playerStatsWindow.Top = (SystemParameters.PrimaryScreenHeight - playerStatsWindow.Height) / 2;

            DoubleAnimation slideInAnimation = new DoubleAnimation(SystemParameters.PrimaryScreenWidth, (SystemParameters.PrimaryScreenWidth - playerStatsWindow.Width) / 2, TimeSpan.FromSeconds(0.3));
            playerStatsWindow.BeginAnimation(Window.LeftProperty, slideInAnimation);

            playerStatsWindow.Show();
        }
    }
}
