using F1Game.Core.Models;

namespace F1Game.Core.Services;

public class TrackGenerator
{
    private readonly Random _random = new();

    public Track Generate(int pointCount, float radius, float variance)
    {
        // 1. Generar puntos iniciales en círculo con ruido
        var rawPoints = GenerateRandomPolygon(pointCount, radius, variance);
        
        // 2. Interpolar con Catmull-Rom para suavizar
        return SmoothToTrack(rawPoints, 10); // 10 segmentos por punto de control
    }

    private List<Vector2D> GenerateRandomPolygon(int count, float radius, float variance)
    {
        var points = new List<Vector2D>();
        float angleStep = (float)(Math.PI * 2 / count);

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;
            float r = radius + (float)((_random.NextDouble() * 2 - 1) * variance);
            points.Add(new Vector2D(
                (float)Math.Cos(angle) * r,
                (float)Math.Sin(angle) * r
            ));
        }

        return points;
    }

    private Track SmoothToTrack(List<Vector2D> controlPoints, int substeps)
    {
        var track = new Track();
        int n = controlPoints.Count;

        for (int i = 0; i < n; i++)
        {
            // Puntos para Catmull-Rom (P0, P1, P2, P3)
            var p0 = controlPoints[(i + n - 1) % n];
            var p1 = controlPoints[i];
            var p2 = controlPoints[(i + 1) % n];
            var p3 = controlPoints[(i + 2) % n];

            for (int j = 0; j < substeps; j++)
            {
                float t = j / (float)substeps;
                float tNext = (j + 1) / (float)substeps;

                var start = GetCatmullRomPoint(p0, p1, p2, p3, t);
                var end = GetCatmullRomPoint(p0, p1, p2, p3, tNext);

                track.Segments.Add(new TrackSegment
                {
                    StartPoint = start,
                    EndPoint = end,
                    Length = start.DistanceTo(end),
                    Inclination = 0 // Podría ser aleatorio también
                });
            }
        }

        return track;
    }

    private Vector2D GetCatmullRomPoint(Vector2D p0, Vector2D p1, Vector2D p2, Vector2D p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float f1 = -0.5f * t3 + t2 - 0.5f * t;
        float f2 = 1.5f * t3 - 2.5f * t2 + 1.0f;
        float f3 = -1.5f * t3 + 2.0f * t2 + 0.5f * t;
        float f4 = 0.5f * t3 - 0.5f * t2;

        return p0 * f1 + p1 * f2 + p2 * f3 + p3 * f4;
    }
}
