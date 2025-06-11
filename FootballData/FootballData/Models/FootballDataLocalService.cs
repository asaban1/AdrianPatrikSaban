using FootballData.DataLayer.Models;
using Newtonsoft.Json;

namespace FootballData.DataLayer
{
    public class FootballDataLocalService : IFootballDataService
    {
        private readonly string _basePath;

        public FootballDataLocalService(string basePath)
        {
            _basePath = basePath;
        }

        public Task<List<Team>> GetTeamsAsync(bool isMen)
        {
            string filePath = GetFilePath(isMen, "results.json");
            return ReadJsonFileAsync<List<Team>>(filePath);
        }

        public Task<List<Match>> GetMatchesAsync(bool isMen)
        {
            string filePath = GetFilePath(isMen, "matches.json");
            return ReadJsonFileAsync<List<Match>>(filePath);
        }

        public async Task<List<Player>> GetPlayersAsync(string championship, string favoriteTeam)
        {
            var matches = await GetMatchesAsync(championship == "men");
            HashSet<Player> loadedPlayers = new HashSet<Player>(new PlayerComparer());

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
                var firstMatch = matches
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

        private string GetFilePath(bool isMen, string fileName)
        {
            string genderFolder = isMen ? "men" : "women";
            return Path.Combine(_basePath, genderFolder, fileName);
        }

        private async Task<T> ReadJsonFileAsync<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
                return new T();

            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<T>(json) ?? new T();
            }
            catch
            {
                return new T();
            }
        }

        private class PlayerComparer : IEqualityComparer<Player>
        {
            public bool Equals(Player x, Player y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(Player obj)
            {
                return obj.Name?.GetHashCode() ?? 0;
            }
        }
        public async Task<Dictionary<string, (int Goals, int YellowCards, int Appearances, int Substitutions)>> GetPlayerStatsAsync(string championship, string favoriteTeam)
        {
            var playerStatsService = new PlayerStatsService(this);
            return await playerStatsService.GetPlayerStatsAsync(championship, favoriteTeam);
        }
        public async Task<List<(string Location, int Attendance, string Home, string Away)>> GetAttendanceRankingAsync(string championship, string favoriteTeam)
        {
            var matches = await GetMatchesAsync(championship == "men");

            var teamMatches = matches
                .Where(m => m.HomeTeam == favoriteTeam || m.AwayTeam == favoriteTeam)
                .Where(m => m.Attendance > 0)
                .OrderByDescending(m => m.Attendance)
                .Select(m => (m.Venue, m.Attendance, m.HomeTeam, m.AwayTeam))
                .ToList();

            return teamMatches;
        }
    }
}
