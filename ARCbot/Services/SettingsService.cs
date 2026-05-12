using System.IO;
using System.Text.Json;
using ARCbot.Helpers;
using ARCbot.Models;

namespace ARCbot.Services;

public class SettingsService
{
    private AppSettings _settings;

    public AppSettings Settings => _settings;

    public SettingsService()
    {
        _settings = LoadSettings();
    }

    public AppSettings LoadSettings()
    {
        if (File.Exists(PathHelper.AppSettingsPath))
        {
            try
            {
                var json = File.ReadAllText(PathHelper.AppSettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "LoadSettings");
                return new AppSettings();
            }
        }
        return new AppSettings();
    }

    public void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PathHelper.AppSettingsPath, json);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "SaveSettings");
        }
    }

    public void UpdateSettings(Action<AppSettings> updateAction)
    {
        updateAction(_settings);
        SaveSettings();
    }
}
