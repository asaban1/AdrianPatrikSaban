using Newtonsoft.Json;

namespace FootballData.DataLayer.Models
{
    public class Player
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "Unknown Player";

        [JsonProperty("shirt_number")]
        public int ShirtNumber { get; set; } = 0;

        [JsonProperty("position")]
        public string Position { get; set; } = "Unknown";

        [JsonProperty("captain")]
        public bool IsCaptain { get; set; }

        public override string ToString()
        {
            return $"{Name} - #{(ShirtNumber > 0 ? ShirtNumber.ToString() : "N/A")} ({Position})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Player other)
            {
                return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase)
                    && this.ShirtNumber == other.ShirtNumber
                    && string.Equals(this.Position, other.Position, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name?.ToLower(), ShirtNumber, Position?.ToLower());
        }
    }
}
