using FootballData.DataLayer.Models;
using Newtonsoft.Json;

namespace FootballData.DataLayer
{
    public class FootballDataService : IFootballDataService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://worldcup-vua.nullbit.hr/";

        public FootballDataService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Team>> GetTeamsAsync(bool isMen)
        {
            string url = isMen ? $"{BaseUrl}men/teams/results" : $"{BaseUrl}women/teams/results";
            return await GetDataAsync<List<Team>>(url);
        }

        public async Task<List<Match>> GetMatchesAsync(bool isMen)
        {
            string url = isMen ? $"{BaseUrl}men/matches" : $"{BaseUrl}women/matches";
            return await GetDataAsync<List<Match>>(url);
        }

        private async Task<T> GetDataAsync<T>(string url) where T : new()
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new T();
                }

                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json) ?? new T();
            }
            catch
            {
                return new T();
            }
        }
        public async Task<List<Player>> GetPlayersAsync(string championship, string favoriteTeam)
        {
            string url = $"{BaseUrl}{championship}/teams/{favoriteTeam}/players";
            var players = await GetDataAsync<List<Player>>(url);

            if (players == null || players.Count == 0)
            {
                var playerService = new PlayerService();
                players = await playerService.GetPlayersAsync(championship, favoriteTeam);
            }

            return players;
        }

        public async Task<Dictionary<string, (int Goals, int YellowCards, int Appearances, int Substitutions)>> GetPlayerStatsAsync(string championship, string favoriteTeam)
        {
            var playerStatsService = new PlayerStatsService(this);
            return await playerStatsService.GetPlayerStatsAsync(championship, favoriteTeam);
        }
        public async Task<List<(string Location, int Attendance, string Home, string Away)>> GetAttendanceRankingAsync(string championship, string favoriteTeam)
        {
            var matchService = new MatchService();
            var matches = await matchService.GetMatchesAsync(championship == "men");

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
