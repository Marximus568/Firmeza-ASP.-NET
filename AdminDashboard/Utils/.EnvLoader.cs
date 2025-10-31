namespace AdminDashboard.Utils;

public static class _EnvLoader
{
    public static void Load(string envFileName = ".env")
    {
        var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), envFileName);

        if (!File.Exists(envFilePath)) return;

        foreach (var line in File.ReadAllLines(envFilePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) 
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}