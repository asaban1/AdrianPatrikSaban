using Newtonsoft.Json;

namespace FootballData.DataLayer.Models
{
    public class Team
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("wins")]
        public int Wins { get; set; }

        [JsonProperty("draws")]
        public int Draws { get; set; }

        [JsonProperty("losses")]
        public int Losses { get; set; }

        [JsonProperty("games_played")]
        public int GamesPlayed { get; set; }

        [JsonProperty("goals_for")]
        public int GoalsFor { get; set; }

        [JsonProperty("goals_against")]
        public int GoalsAgainst { get; set; }

        public override string ToString()
        {
            return $"{Country} ({Id})";
        }
    }
}
