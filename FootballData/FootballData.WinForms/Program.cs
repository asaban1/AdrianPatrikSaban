namespace FootballData.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FootballData",
                "settings.json"
            );

            if (!File.Exists(settingsPath))
            {
                using (var settingsForm = new SettingsForm())
                {
                    var result = settingsForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        Application.Run(new MainForm());
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                Application.Run(new MainForm());
            }
        }
    }
}
