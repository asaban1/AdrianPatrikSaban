using FootballData.DataLayer.Models;

namespace FootballData.DataLayer
{
    public class MatchService
    {
        private readonly IFootballDataService _service;

        public MatchService()
        {
            _service = new FootballDataService();
        }

        public MatchService(IFootballDataService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<List<Match>> GetMatchesAsync(bool isMen)
        {
            return await _service.GetMatchesAsync(isMen);
        }
    }
}
