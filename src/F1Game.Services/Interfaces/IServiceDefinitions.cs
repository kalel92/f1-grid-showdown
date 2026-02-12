namespace F1Game.Services.Interfaces;

public interface IAudioManager
{
    void PlaySound(string soundName, float volume = 1.0f);
    void PlayMusic(string musicName, bool loop = true);
    void SetMasterVolume(float volume);
}

public interface IInputManager
{
    float GetAxis(string axisName);
    bool GetButtonDown(string buttonName);
    void Update();
}

public interface IUserDataService
{
    UserProfile GetOrCreateProfile(string username);
    void SaveProfile(UserProfile profile);
}

public interface IConfigService
{
    GameSettings GetSettings();
    void SaveSettings(GameSettings settings);
}

public class UserProfile
{
    public string Username { get; set; } = string.Empty;
    public float BestLapTime { get; set; } = float.MaxValue;
    public decimal AccumulatedMoney { get; set; } = 0;
}

public class GameSettings
{
    public float AudioVolume { get; set; } = 0.8f;
    public Dictionary<string, string> KeyBindings { get; set; } = new();
}
