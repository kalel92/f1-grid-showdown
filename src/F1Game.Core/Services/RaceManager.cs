using F1Game.Core.Models;

namespace F1Game.Core.Services;

public class RaceManager
{
    private readonly PhysicsEngine _physics = new();
    private readonly List<AICar> _opponents = new();
    
    public IEnumerable<AICar> Opponents => _opponents;

    public void InitializeOpponents(int count, Track track)
    {
        var random = new Random();
        for (int i = 0; i < count; i++)
        {
            _opponents.Add(new AICar
            {
                MaxSpeed = 330.0f + random.Next(0, 30),
                BaseAcceleration = 40.0f + random.Next(0, 10),
                IdealLateralOffset = (float)(random.NextDouble() * 1.6 - 0.8), // Distribuidos en la pista
                Position = new TrackPosition { Progress = (i + 1) * 50, LateralOffset = 0 },
                Skill = (float)random.NextDouble() * 0.5f + 0.5f,
                Aggression = (float)random.NextDouble()
            });
        }
    }

    public void Update(float deltaTime, Track track)
    {
        foreach (var ai in _opponents)
        {
            // 1. Decidir Input de la IA
            float gas = 1.0f;
            float brake = 0.0f;
            float steering = 0.0f;

            // Simple IA: Seguir línea de carrera
            float diffX = ai.IdealLateralOffset - ai.Position.LateralOffset;
            steering = Math.Clamp(diffX * 2.0f, -1.0f, 1.0f);

            // Frenado básico en "curvas" (basado en la curvatura de los siguientes segmentos)
            float curvatureAhead = GetCurvatureAhead(ai.Position.Progress, track, 10);
            if (curvatureAhead > 0.5f && ai.CurrentSpeed > 150)
            {
                gas = 0.2f;
                brake = 0.8f * ai.Aggression;
            }

            // 2. Aplicar Física
            _physics.Update(ai, deltaTime, gas, steering, brake);

            // 3. Loop de pista
            if (ai.Position.Progress > track.TotalLength)
                ai.Position = ai.Position with { Progress = ai.Position.Progress - track.TotalLength };
        }
    }

    private float GetCurvatureAhead(float progress, Track track, int segmentCount)
    {
        // Simplificado: suma de diferencias de X en los siguientes segmentos
        // En una implementación real, analizaríamos el ángulo total
        return 0.1f; // Placeholder para control de velocidad IA
    }
}
