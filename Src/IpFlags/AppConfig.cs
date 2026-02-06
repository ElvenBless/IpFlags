using System.Text.Json;

namespace IpFlags;

static class AppConfig
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IpFlags", "config.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static bool PollingEnabled
    {
        get => Load().PollingEnabled;
        set
        {
            var c = Load();
            c.PollingEnabled = value;
            Save(c);
        }
    }

    /// <summary>True = UpdateIcon on DoubleClick, False = on Click.</summary>
    public static bool UpdateOnDoubleClick
    {
        get => Load().UpdateOnDoubleClick;
        set
        {
            var c = Load();
            c.UpdateOnDoubleClick = value;
            Save(c);
        }
    }

    private static ConfigRecord Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var record = JsonSerializer.Deserialize<ConfigRecord>(json);
                if (record != null) return record;
            }
        }
        catch { /* ignore */ }

        return new ConfigRecord { PollingEnabled = true, UpdateOnDoubleClick = true };
    }

    private static void Save(ConfigRecord record)
    {
        try
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(record, JsonOptions));
        }
        catch { /* ignore */ }
    }

    private class ConfigRecord
    {
        public bool PollingEnabled { get; set; } = true;
        public bool UpdateOnDoubleClick { get; set; } = true;
    }
}
