namespace AdminDashboardApplication.Common;

public static class EnvLoader
{
    public static void Load(string envFileName = ".env")
    {
        // Find the .env file searching upward from the current directory
        var envPath = FindFileUpwards(Directory.GetCurrentDirectory(), envFileName);
        if (envPath == null) return;

        foreach (var raw in File.ReadAllLines(envPath))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("#") || line.StartsWith("//")) continue;

            var parts = line.Split('=', 2);
            if (parts.Length != 2) continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim().Trim('"', '\'');

            // Do not overwrite existing environment variables
            if (Environment.GetEnvironmentVariable(key) == null)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    private static string? FindFileUpwards(string startDir, string fileName)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, fileName);
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        return null;
    }
}