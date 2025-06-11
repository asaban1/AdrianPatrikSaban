using Newtonsoft.Json;

namespace FootballData.DataLayer.Models
{
    public class FavoritePlayerService
    {
        private const string FavoritePlayersFile = "favorite_players.txt";

        public List<Player> LoadFavoritePlayers()
        {
            if (!File.Exists(FavoritePlayersFile))
                return new List<Player>();

            string json = File.ReadAllText(FavoritePlayersFile);
            return JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
        }

        public void SaveFavoritePlayers(List<Player> favoritePlayers)
        {
            string json = JsonConvert.SerializeObject(favoritePlayers, Formatting.Indented);
            File.WriteAllText(FavoritePlayersFile, json);
        }
    }
}
