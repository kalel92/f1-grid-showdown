namespace F1Game.Core.Models;

public record Vector2D(float X, float Y)
{
    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2D operator -(Vector2D a, Vector2D b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2D operator *(Vector2D a, float b) => new(a.X * b, a.Y * b);
    public static Vector2D operator *(float a, Vector2D b) => new(a * b.X, a * b.Y);

    public float DistanceTo(Vector2D other)
    {
        float dx = X - other.X;
        float dy = Y - other.Y;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    public float Length() => (float)Math.Sqrt(X * X + Y * Y);
}

public class TrackSegment
{
    public Vector2D StartPoint { get; set; } = new(0, 0);
    public Vector2D EndPoint { get; set; } = new(0, 0);
    public float Length { get; set; }
    public float Inclination { get; set; } // Grados o pendiente
    public Vector2D ControlPoint { get; set; } = new(0, 0); // Para curvas
}

public class Track
{
    public List<TrackSegment> Segments { get; set; } = new();
    public float TotalLength => Segments.Sum(s => s.Length);
}
