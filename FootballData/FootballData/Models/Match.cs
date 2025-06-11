using Newtonsoft.Json;

namespace FootballData.DataLayer.Models
{
    public class Match
    {
        [JsonProperty("venue")]
        public string Venue { get; set; }

        [JsonProperty("home_team_country")]
        public string HomeTeam { get; set; }

        [JsonProperty("away_team_country")]
        public string AwayTeam { get; set; }

        [JsonProperty("datetime")]
        public DateTime Date { get; set; }

        [JsonProperty("winner")]
        public string Winner { get; set; }

        [JsonProperty("home_team")]
        public TeamScore HomeTeamScore { get; set; }

        [JsonProperty("away_team")]
        public TeamScore AwayTeamScore { get; set; }

        [JsonProperty("home_team_statistics")]
        public TeamStatistics HomeTeamStatistics { get; set; }

        [JsonProperty("away_team_statistics")]
        public TeamStatistics AwayTeamStatistics { get; set; }

        [JsonProperty("home_team_events")]
        public List<MatchEvent> HomeTeamEvents { get; set; } = new List<MatchEvent>();

        [JsonProperty("away_team_events")]
        public List<MatchEvent> AwayTeamEvents { get; set; } = new List<MatchEvent>();

        [JsonProperty("attendance")]
        public int Attendance { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public override string ToString()
        {
            return $"{Date:dd.MM.yyyy} | {HomeTeam} {HomeTeamScore.Goals} - {AwayTeamScore.Goals} {AwayTeam} | 🏆 Winner: {Winner} | 📍 Stadium: {Venue} | 👥 Attendance: {Attendance}";
        }
    }

    public class TeamStatistics
    {
        [JsonProperty("starting_eleven")]
        public List<Player> StartingEleven { get; set; } = new List<Player>();

        [JsonProperty("substitutes")]
        public List<Player> Substitutes { get; set; } = new List<Player>();
    }

    public class MatchEvent
    {
        [JsonProperty("type_of_event")]
        public string TypeOfEvent { get; set; }

        [JsonProperty("player")]
        public string Player { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        public override string ToString()
        {
            return $"{Time} - {Player} ({TypeOfEvent})";
        }
    }
}
