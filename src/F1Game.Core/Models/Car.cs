namespace F1Game.Core.Models;

public class Car
{
    // Rendimiento base
    public float MaxSpeed { get; set; } = 350.0f; // km/h
    public float BaseAcceleration { get; set; } = 40.0f; // m/s^2
    public float BaseHandling { get; set; } = 1.0f; // Multiplicador de agarre

    // Estado dinámico
    public float CurrentSpeed { get; set; } = 0.0f;
    public TrackPosition Position { get; set; } = new TrackPosition();

    // Propiedades físicas calculadas
    public bool IsGrounded { get; set; } = true;
}

public struct TrackPosition
{
    public float Progress { get; set; } // Progreso longitudinal en la pista (m)
    public float LateralOffset { get; set; } // Posición transversal (X) relativa al centro
}
