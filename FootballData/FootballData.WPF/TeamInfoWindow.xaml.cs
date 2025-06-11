using System.Windows;
using System.Windows.Media.Animation;

namespace FootballData.WPF
{
    public partial class TeamInfoWindow : Window
    {
        public TeamInfoWindow(string teamName, string fifaCode, int matchesPlayed, int wins, int losses, int draws, int goalsScored, int goalsConceded)
        {
            InitializeComponent();
            txtTeamName.Text = teamName;
            txtFifaCode.Text = fifaCode;
            txtMatchesPlayed.Text = matchesPlayed.ToString();
            txtWins.Text = wins.ToString();
            txtLosses.Text = losses.ToString();
            txtDraws.Text = draws.ToString();
            txtGoalsScored.Text = goalsScored.ToString();
            txtGoalsConceded.Text = goalsConceded.ToString();
            AnimateWindowOpening();
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        private void AnimateWindowOpening()
        {
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            this.BeginAnimation(Window.OpacityProperty, fadeIn);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
