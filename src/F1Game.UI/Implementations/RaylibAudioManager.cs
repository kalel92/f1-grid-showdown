using F1Game.Services.Interfaces;
using Raylib_cs;

namespace F1Game.UI.Implementations;

public class RaylibAudioManager : IAudioManager
{
    private Dictionary<string, Sound> _sounds = new();
    private Dictionary<string, Music> _music = new();

    public RaylibAudioManager()
    {
        Raylib.InitAudioDevice();
    }

    public void LoadSound(string name, string path)
    {
        _sounds[name] = Raylib.LoadSound(path);
    }

    public void PlaySound(string soundName, float volume = 1.0f)
    {
        if (_sounds.TryGetValue(soundName, out var sound))
        {
            Raylib.SetSoundVolume(sound, volume);
            Raylib.PlaySound(sound);
        }
    }

    public void UpdateEnginePitch(string soundName, float speed, float maxSpeed)
    {
        if (_sounds.TryGetValue(soundName, out var sound))
        {
            // El pitch varía entre 0.5 (reposo) y 2.5 (máxima velocidad)
            float pitch = 0.5f + (speed / maxSpeed) * 2.0f;
            Raylib.SetSoundPitch(sound, pitch);
            if (!Raylib.IsSoundPlaying(sound)) Raylib.PlaySound(sound);
        }
    }

    public void PlayMusic(string musicName, bool loop = true) { /* Implementar similar a Sound */ }
    public void SetMasterVolume(float volume) => Raylib.SetMasterVolume(volume);

    public void Dispose()
    {
        foreach (var s in _sounds.Values) Raylib.UnloadSound(s);
        Raylib.CloseAudioDevice();
    }
}
