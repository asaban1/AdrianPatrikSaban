using FootballData.DataLayer.Models;

namespace FootballData.DataLayer
{
    public class AttendanceRankingService
    {
        private readonly MatchService _matchService;

        public AttendanceRankingService()
        {
            _matchService = new MatchService();
        }

        public async Task<List<(string Location, int Attendance, string Home, string Away)>> GetAttendanceRankingAsync(string championship, string favoriteTeam)
        {
            List<Match> matches = await _matchService.GetMatchesAsync(championship == "men");

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
