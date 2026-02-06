namespace IpFlags;

public class FlagImageService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
    private readonly string _flagsDir;
    private readonly object _lock = new();

    private const string BaseUrl = "https://flagcdn.com/24x18/";

    public FlagImageService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _flagsDir = Path.Combine(appData, "IpFlags");
        lock (_lock)
        {
            if (!Directory.Exists(_flagsDir))
                Directory.CreateDirectory(_flagsDir);
        }
    }

    public async Task<string?> GetFlagImagePathAsync(string flagCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(flagCode)) return null;
        var code = flagCode.Trim().ToLowerInvariant();
        if (code.Length != 2) return null;

        var path = Path.Combine(_flagsDir, $"{code}.png");
        lock (_lock)
        {
            if (File.Exists(path)) return path;
        }

        try
        {
            var url = BaseUrl + code + ".png";
            var bytes = await HttpClient.GetByteArrayAsync(url, ct).ConfigureAwait(false);
            await File.WriteAllBytesAsync(path, bytes, ct).ConfigureAwait(false);
            return path;
        }
        catch
        {
            return null;
        }
    }
}