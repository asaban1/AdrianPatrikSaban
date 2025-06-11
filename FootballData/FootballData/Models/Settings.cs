using Newtonsoft.Json;

public class Settings
{
    public string Championship { get; set; } = "men";
    public string Language { get; set; } = "en";
    public string FavoriteTeam { get; set; } = "";
    public List<string> FavoritePlayers { get; set; } = new List<string>();
    public string WindowSize { get; set; } = "1280x720";
    public string DataSource { get; set; } = "API";

    private static readonly string SettingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FootballData",
        "settings.json"
    );

    public static Settings Load()
    {
        string directoryPath = Path.GetDirectoryName(SettingsFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(SettingsFilePath))
        {
            try
            {
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFilePath)) ?? new Settings();
            }
            catch
            {
                return new Settings();
            }
        }

        return new Settings();
    }

    public void Save()
    {
        File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}
