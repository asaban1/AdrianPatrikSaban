namespace FootballData.DataLayer.Models
{
    public static class MatchEventSymbolProvider
    {
        private static readonly Dictionary<string, string> _symbols = new()
    {
        { "yellow-card", "🟨 (YC)" },
        { "goal", "⚽ (G)" },
        { "goal-penalty", "⚽️ (PG)" },
        { "goal-own", "❌ (OG)" },
        { "substitution-in", "⬆️ (IN)" },
        { "substitution-out", "⬇️ (OUT)" }
    };

        public static string GetSymbol(string eventType)
        {
            return _symbols.TryGetValue(eventType, out var symbol) ? symbol : eventType;
        }
    }
}
