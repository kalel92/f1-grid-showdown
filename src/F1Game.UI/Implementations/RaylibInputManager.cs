using F1Game.Services.Interfaces;
using Raylib_cs;

namespace F1Game.UI.Implementations;

public class RaylibInputManager : IInputManager
{
    public float GetAxis(string axisName)
    {
        if (axisName == "Vertical")
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W) || Raylib.IsKeyDown(KeyboardKey.KEY_UP)) return 1.0f;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_S) || Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) return -1.0f;
        }
        
        if (axisName == "Horizontal")
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) return 1.0f;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) return -1.0f;
        }

        return 0.0f;
    }

    public bool GetButtonDown(string buttonName)
    {
        if (buttonName == "Jump") return Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE);
        return false;
    }

    public void Update() { /* Raylib actualiza el estado internamente */ }
}
