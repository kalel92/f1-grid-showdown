using F1Game.Core.Models;
using F1Game.Core.Services;
using F1Game.Core.Rendering;
using F1Game.Services.Interfaces;
using F1Game.Services.Implementations;
using System.Diagnostics;

namespace F1Game.UI.GameLoop;

public class GameApp
{
    private readonly PhysicsEngine _physics = new();
    private readonly TrackGenerator _generator = new();
    private readonly Pseudo3DCamera _camera = new();
    private readonly Car _playerCar = new();
    private readonly Track _track;
    
    private readonly IInputManager _input = new MockInputManager(); // Simulado para este ejemplo
    private bool _isRunning = true;

    public GameApp()
    {
        _track = _generator.Generate(20, 800, 100); // Generar pista inicial
        _playerCar.MaxSpeed = 350.0f;
    }

    public void Run()
    {
        const double targetFps = 60.0;
        const double msPerFrame = 1000.0 / targetFps;
        Stopwatch sw = Stopwatch.StartNew();
        double lastTime = 0;

        Console.WriteLine("F1 Game Engine Started. Running at 60 FPS...");

        while (_isRunning)
        {
            double currentTime = sw.Elapsed.TotalMilliseconds;
            double elapsed = currentTime - lastTime;

            if (elapsed >= msPerFrame)
            {
                float deltaTime = (float)elapsed / 1000.0f;
                Update(deltaTime);
                Render();
                lastTime = currentTime;
            }

            // Evitar consumo excesivo de CPU en el loop
            if (msPerFrame - (sw.Elapsed.TotalMilliseconds - lastTime) > 1)
                Thread.Sleep(1);
        }
    }

    private void Update(float deltaTime)
    {
        // 1. Obtener Input (En un motor real esto vendría de SDL2, Monogame o Unity)
        float gas = _input.GetAxis("Vertical") > 0 ? 1.0f : 0.0f;
        float brake = _input.GetAxis("Vertical") < 0 ? 1.0f : 0.0f;
        float steering = _input.GetAxis("Horizontal");

        // 2. Actualizar Física
        _physics.Update(_playerCar, deltaTime, gas, steering, brake);

        // 3. Loop de Pista (Si llega al final, reinicia progreso para simular circuito cerrado)
        if (_playerCar.Position.Progress > _track.TotalLength)
        {
            _playerCar.Position = _playerCar.Position with { Progress = 0 };
        }
    }

    private void Render()
    {
        // Aquí llamaríamos al TrackRenderer.Draw
        // Por ahora, simulamos la proyección para validar la lógica
        var screenPolygons = _camera.Project(_track, _playerCar, 800, 600, 10, 0.5f);
        
        // Console.Write($"\rSpeed: {_playerCar.CurrentSpeed:F1} km/h | Progress: {_playerCar.Position.Progress:F0}m | Polys: {screenPolygons.Count}   ");
    }
}
