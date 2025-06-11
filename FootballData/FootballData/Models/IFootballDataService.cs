using FootballData.DataLayer.Models;

namespace FootballData.DataLayer
{
    public interface IFootballDataService
    {
        Task<List<Team>> GetTeamsAsync(bool isMen);
        Task<List<Match>> GetMatchesAsync(bool isMen);
        Task<List<Player>> GetPlayersAsync(string championship, string favoriteTeam);
        Task<Dictionary<string, (int Goals, int YellowCards, int Appearances, int Substitutions)>> GetPlayerStatsAsync(string championship, string favoriteTeam);
        Task<List<(string Location, int Attendance, string Home, string Away)>> GetAttendanceRankingAsync(string championship, string favoriteTeam);
    }
}
