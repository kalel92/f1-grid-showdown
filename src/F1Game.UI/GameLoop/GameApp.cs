using F1Game.Core.Models;
using F1Game.Core.Services;
using F1Game.Core.Rendering;
using F1Game.Services.Interfaces;
using F1Game.UI.Rendering;
using F1Game.UI.Implementations;
using Raylib_cs;

namespace F1Game.UI.GameLoop;

public class GameApp
{
    private readonly PhysicsEngine _physics = new();
    private readonly TrackGenerator _generator = new();
    private readonly Pseudo3DCamera _camera = new();
    private readonly TrackRenderer _renderer = new();
    private readonly RaceManager _raceManager = new();
    private readonly RaylibAudioManager _audio = new();
    
    private readonly Car _playerCar = new();
    private readonly Track _track;
    private readonly IInputManager _input;

    public GameApp()
    {
        _track = _generator.Generate(30, 1000, 150);
        _raceManager.InitializeOpponents(5, _track);
        
        _playerCar.MaxSpeed = 350.0f;
        _playerCar.BaseAcceleration = 50.0f;
        _input = new RaylibInputManager();
    }

    public void Run()
    {
        // Inicializar Ventana
        Raylib.InitWindow(1280, 720, "F1 Grid Showdown - Prototype");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();
            
            Update(deltaTime);
            
            Raylib.BeginDrawing();
            Render();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private void Update(float deltaTime)
    {
        float gas = _input.GetAxis("Vertical") > 0 ? 1.0f : 0.0f;
        float brake = _input.GetAxis("Vertical") < 0 ? 1.0f : 0.0f;
        float steering = _input.GetAxis("Horizontal");

        _physics.Update(_playerCar, deltaTime, gas, steering, brake);
        _raceManager.Update(deltaTime, _track);

        // Audio: Simular motor (Agnóstico a si el archivo existe por ahora)
        _audio.UpdateEnginePitch("engine", _playerCar.CurrentSpeed, _playerCar.MaxSpeed);

        // Loop infinito de pista
        if (_playerCar.Position.Progress > _track.TotalLength)
            _playerCar.Position = _playerCar.Position with { Progress = _playerCar.Position.Progress - _track.TotalLength };
    }

    private void Render()
    {
        // Proyectar segmentos visibles
        var screenPolygons = _camera.Project(_track, _playerCar, 1280, 720, 15, 0.7f, 200);
        
        // Dibujar frame con oponentes
        _renderer.DrawFrame(screenPolygons, _input.GetAxis("Horizontal"), _raceManager.Opponents, _camera, _playerCar);

        // UI de telemetría
        Raylib.DrawRectangle(10, 10, 250, 80, Raylib.Fade(Color.BLACK, 0.5f));
        Raylib.DrawText($"SPEED: {(int)_playerCar.CurrentSpeed} KM/H", 20, 20, 20, Color.YELLOW);
        Raylib.DrawText($"PROGRESS: {(int)_playerCar.Position.Progress} M", 20, 45, 20, Color.WHITE);
        Raylib.DrawText($"FPS: {Raylib.GetFPS()}", 20, 70, 15, Color.GREEN);
    }
}
