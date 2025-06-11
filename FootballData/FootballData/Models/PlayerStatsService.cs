using FootballData.DataLayer.Models;

namespace FootballData.DataLayer
{
    public class PlayerStatsService
    {
        private readonly MatchService _matchService;
        private readonly IFootballDataService _dataService;

        public PlayerStatsService(IFootballDataService dataService)
        {
            _dataService = dataService;
        }

        public PlayerStatsService()
        {
            _matchService = new MatchService();
        }

        public async Task<Dictionary<string, (int Goals, int YellowCards, int Appearances, int Substitutions)>> GetPlayerStatsAsync(string championship, string favoriteTeam)
        {
            List<Match> matches = await _dataService.GetMatchesAsync(championship == "men");
            var playerStats = new Dictionary<string, (int Goals, int YellowCards, int Appearances, int Substitutions)>();

            foreach (var match in matches)
            {
                bool isFavoriteTeamHome = match.HomeTeam == favoriteTeam;
                bool isFavoriteTeamAway = match.AwayTeam == favoriteTeam;

                if (!isFavoriteTeamHome && !isFavoriteTeamAway)
                    continue;

                var teamEvents = isFavoriteTeamHome ? match.HomeTeamEvents : match.AwayTeamEvents;

                if (teamEvents == null || teamEvents.Count == 0)
                    continue;

                var playersInMatch = new HashSet<string>();

                var startingPlayers = isFavoriteTeamHome ? match.HomeTeamStatistics.StartingEleven : match.AwayTeamStatistics.StartingEleven;
                var substitutes = isFavoriteTeamHome ? match.HomeTeamStatistics.Substitutes : match.AwayTeamStatistics.Substitutes;

                foreach (var player in startingPlayers)
                {
                    if (!playerStats.ContainsKey(player.Name))
                    {
                        playerStats[player.Name] = (0, 0, 0, 0);
                    }

                    var (goals, yellowCards, appearances, substitutions) = playerStats[player.Name];
                    appearances++;
                    playerStats[player.Name] = (goals, yellowCards, appearances, substitutions);
                }

                foreach (var ev in teamEvents.Where(e => e.TypeOfEvent == "substitution-in"))
                {
                    if (!string.IsNullOrEmpty(ev.Player))
                    {
                        if (!playerStats.ContainsKey(ev.Player))
                        {
                            playerStats[ev.Player] = (0, 0, 0, 0);
                        }

                        var (goals, yellowCards, appearances, substitutions) = playerStats[ev.Player];
                        substitutions++;
                        appearances++;
                        playerStats[ev.Player] = (goals, yellowCards, appearances, substitutions);
                    }
                }

                foreach (var ev in teamEvents)
                {
                    if (string.IsNullOrEmpty(ev.Player)) continue;

                    if (!playerStats.ContainsKey(ev.Player))
                    {
                        playerStats[ev.Player] = (0, 0, 0, 0);
                    }

                    var (goals, yellowCards, appearances, substitutions) = playerStats[ev.Player];

                    if (ev.TypeOfEvent == "goal")
                    {
                        goals++;
                    }
                    else if (ev.TypeOfEvent == "yellow-card")
                    {
                        yellowCards++;
                    }
                    playerStats[ev.Player] = (goals, yellowCards, appearances, substitutions);
                }
            }
            return playerStats;
        }

        public async Task<List<MatchEvent>> GetMatchEventsForPlayer(Player player)
        {
            List<Match> matches = await _matchService.GetMatchesAsync(true);

            return matches
                .SelectMany(m => m.HomeTeamEvents.Concat(m.AwayTeamEvents))
                .Where(e => e.Player == player.Name &&
                           (e.TypeOfEvent == "goal" ||
                            e.TypeOfEvent == "goal-penalty" ||
                            e.TypeOfEvent == "yellow-card"))
                .ToList();
        }
    }
}