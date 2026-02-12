using Xunit;
using F1Game.Core.Services;
using F1Game.Core.Models;

namespace F1Game.Tests;

public class TrackGeneratorTests
{
    private readonly TrackGenerator _generator;

    public TrackGeneratorTests()
    {
        _generator = new TrackGenerator();
    }

    [Fact]
    public void Generate_ShouldBeClosedCircuit()
    {
        // Arrange
        int points = 10;
        
        // Act
        var track = _generator.Generate(points, 500, 50);
        var firstPoint = track.Segments.First().StartPoint;
        var lastPoint = track.Segments.Last().EndPoint;

        // Assert
        float distance = firstPoint.DistanceTo(lastPoint);
        Assert.True(distance < 0.1f, $"La pista no está cerrada. Delta: {distance}");
    }

    [Fact]
    public void Generate_TotalLength_ShouldBePositive()
    {
        // Act
        var track = _generator.Generate(12, 1000, 100);

        // Assert
        Assert.True(track.TotalLength > 0);
    }

    [Fact]
    public void Generate_ShouldNotHaveSharpAngles()
    {
        // Arrange
        var track = _generator.Generate(15, 800, 100);
        
        // Act & Assert
        for (int i = 0; i < track.Segments.Count; i++)
        {
            var current = track.Segments[i];
            var next = track.Segments[(i + 1) % track.Segments.Count];

            var dir1 = (current.EndPoint - current.StartPoint);
            var dir2 = (next.EndPoint - next.StartPoint);

            // Calcular el ángulo entre segmentos consecutivos
            float dot = (dir1.X * dir2.X + dir1.Y * dir2.Y) / (dir1.Length() * dir2.Length());
            
            // Dot product de 1 es mismo sentido, 0 es 90 grados, -1 opuesto.
            // Para F1, un ángulo de menos de 30 grados entre micro-segmentos es razonable (dot > 0.8)
            // Considerando que son curvas suaves por Splines, el dot debería ser muy cercano a 1.
            Assert.True(dot > 0.95f, $"Ángulo demasiado agudo en segmento {i}. Dot: {dot}");
        }
    }
}
