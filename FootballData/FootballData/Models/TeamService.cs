using FootballData.DataLayer.Models;

namespace FootballData.DataLayer
{
    public class TeamService
    {
        private readonly FootballDataService _service;

        public TeamService()
        {
            _service = new FootballDataService();
        }

        public async Task<List<Team>> GetTeamsAsync(bool isMen)
        {
            return await _service.GetTeamsAsync(isMen);
        }
    }
}
