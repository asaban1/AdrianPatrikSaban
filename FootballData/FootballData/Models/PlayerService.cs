using FootballData.DataLayer.Models;

namespace FootballData.DataLayer
{
    public class PlayerService
    {
        private readonly MatchService _matchService;

        public PlayerService()
        {
            _matchService = new MatchService();
        }

        public async Task<List<Player>> GetPlayersAsync(string championship, string favoriteTeam)
        {
            List<Match> matches = await _matchService.GetMatchesAsync(championship == "men");
            HashSet<Player> loadedPlayers = new HashSet<Player>();

            if (string.IsNullOrEmpty(favoriteTeam))
            {
                foreach (var match in matches)
                {
                    if (match.HomeTeamStatistics?.StartingEleven != null)
                        loadedPlayers.UnionWith(match.HomeTeamStatistics.StartingEleven);
                    if (match.HomeTeamStatistics?.Substitutes != null)
                        loadedPlayers.UnionWith(match.HomeTeamStatistics.Substitutes);
                    if (match.AwayTeamStatistics?.StartingEleven != null)
                        loadedPlayers.UnionWith(match.AwayTeamStatistics.StartingEleven);
                    if (match.AwayTeamStatistics?.Substitutes != null)
                        loadedPlayers.UnionWith(match.AwayTeamStatistics.Substitutes);
                }
            }
            else
            {
                Match firstMatch = matches
                    .Where(m => m.HomeTeam == favoriteTeam || m.AwayTeam == favoriteTeam)
                    .OrderBy(m => m.Date)
                    .FirstOrDefault();

                if (firstMatch != null)
                {
                    if (firstMatch.HomeTeam == favoriteTeam)
                    {
                        loadedPlayers.UnionWith(firstMatch.HomeTeamStatistics?.StartingEleven ?? new List<Player>());
                        loadedPlayers.UnionWith(firstMatch.HomeTeamStatistics?.Substitutes ?? new List<Player>());
                    }
                    else
                    {
                        loadedPlayers.UnionWith(firstMatch.AwayTeamStatistics?.StartingEleven ?? new List<Player>());
                        loadedPlayers.UnionWith(firstMatch.AwayTeamStatistics?.Substitutes ?? new List<Player>());
                    }
                }
            }

            return loadedPlayers.ToList();
        }
    }
}
