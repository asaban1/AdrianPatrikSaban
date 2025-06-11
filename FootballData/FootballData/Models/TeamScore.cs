using Newtonsoft.Json;

namespace FootballData.DataLayer.Models
{
    public class TeamScore
    {
        [JsonProperty("starting_eleven")]
        public List<Player> StartingEleven { get; set; } = new List<Player>();

        [JsonProperty("substitutes")]
        public List<Player> Substitutes { get; set; } = new List<Player>();

        [JsonProperty("goals")]
        public int Goals { get; set; }

        public override string ToString()
        {
            return $"Goals: {Goals}, Starting Eleven: {StartingEleven.Count}, Substitutes: {Substitutes.Count}";
        }
    }
}
