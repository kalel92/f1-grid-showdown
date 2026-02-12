using System.Text.Json;
using F1Game.Services.Interfaces;

namespace F1Game.Services.Implementations;

public class UserDataService : IUserDataService
{
    private readonly string _savePath;

    public UserDataService(string savePath = "userdata")
    {
        _savePath = savePath;
        if (!Directory.Exists(_savePath)) Directory.CreateDirectory(_savePath);
    }

    public UserProfile GetOrCreateProfile(string username)
    {
        string filePath = Path.Combine(_savePath, $"{username}.json");
        if (!File.Exists(filePath))
        {
            return new UserProfile { Username = username };
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<UserProfile>(json) ?? new UserProfile { Username = username };
    }

    public void SaveProfile(UserProfile profile)
    {
        string filePath = Path.Combine(_savePath, $"{profile.Username}.json");
        string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

public class ConfigService : IConfigService
{
    private readonly string _configPath;

    public ConfigService(string configPath = "config.json")
    {
        _configPath = configPath;
    }

    public GameSettings GetSettings()
    {
        if (!File.Exists(_configPath)) return new GameSettings();
        string json = File.ReadAllText(_configPath);
        return JsonSerializer.Deserialize<GameSettings>(json) ?? new GameSettings();
    }

    public void SaveSettings(GameSettings settings)
    {
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }
}

public class MockAudioManager : IAudioManager
{
    public void PlaySound(string soundName, float volume = 1.0f) { /* Mock */ }
    public void PlayMusic(string musicName, bool loop = true) { /* Mock */ }
    public void SetMasterVolume(float volume) { /* Mock */ }
}

public class MockInputManager : IInputManager
{
    public float GetAxis(string axisName) => 0.0f;
    public bool GetButtonDown(string buttonName) => false;
    public void Update() { /* Mock */ }
}
