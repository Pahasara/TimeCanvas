using System.Text.Json;
using TimeCanvas.Core.Models;

namespace TimeCanvas.Core.Services;

public class SettingsService
{
    private readonly string _path = Path.Combine(
        Path.GetDirectoryName(AppPaths.DatabaseFile)!, "settings.json");

    private AppSettings? _cache;

    public async Task<AppSettings> LoadAsync()
    {
        if (_cache is not null) return _cache;

        if (!File.Exists(_path))
        {
            _cache = new AppSettings();
            return _cache;
        }

        await using var stream = File.OpenRead(_path);
        _cache = await JsonSerializer.DeserializeAsync(stream, AppSettingsJsonContext.Default.AppSettings)
                 ?? new AppSettings();
        return _cache;
    }

    public async Task SaveAsync(AppSettings settings)
    {
        _cache = settings;
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, settings, AppSettingsJsonContext.Default.AppSettings);
    }
}
