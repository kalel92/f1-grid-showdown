using Xunit;
using F1Game.Core.Rendering;
using F1Game.Core.Models;

namespace F1Game.Tests;

public class Pseudo3DCameraTests
{
    private readonly Pseudo3DCamera _camera;
    private readonly Track _mockTrack;
    private readonly Car _mockCar;

    public Pseudo3DCameraTests()
    {
        _camera = new Pseudo3DCamera();
        _mockCar = new Car { Position = new TrackPosition { Progress = 0, LateralOffset = 0 } };
        
        // Crear una pista recta simple para pruebas
        _mockTrack = new Track();
        for (int i = 0; i < 100; i++)
        {
            _mockTrack.Segments.Add(new TrackSegment 
            { 
                Length = 10,
                StartPoint = new Vector2D(0, i * 10),
                EndPoint = new Vector2D(0, (i + 1) * 10)
            });
        }
    }

    [Fact]
    public void Project_FarSegments_ShouldHaveSmallerScaleThanNearSegments()
    {
        // Act
        var result = _camera.Project(_mockTrack, _mockCar, 800, 600, 10, 0.5f);

        // Dado que invertimos la lista para el Painter's Algorithm, 
        // el primer elemento es el más LEJANO y el último es el más CERCANO.
        var farSegment = result.First();
        var nearSegment = result.Last();

        // Assert
        Assert.True(farSegment.LowerLeft.Scale < nearSegment.LowerLeft.Scale, 
            $"El segmento lejano ({farSegment.LowerLeft.Scale}) debería tener menor escala que el cercano ({nearSegment.LowerLeft.Scale})");
    }

    [Fact]
    public void Project_SegmentsShouldConvergeToHorizon()
    {
        // Horizon is screenHeight / 2 (300 in this case)
        int screenHeight = 600;
        float horizon = screenHeight / 2.0f;

        // Act
        var result = _camera.Project(_mockTrack, _mockCar, 800, screenHeight, 10, 0.5f);

        var farSegment = result.First();
        var nearSegment = result.Last();

        // Diferencia absoluta con el horizonte
        float farDiff = Math.Abs(farSegment.UpperLeft.Y - horizon);
        float nearDiff = Math.Abs(nearSegment.LowerLeft.Y - horizon);

        // Assert
        Assert.True(farDiff < nearDiff, 
            $"El segmento lejano ({farSegment.UpperLeft.Y}) debería estar más cerca del horizonte (300) que el cercano ({nearSegment.LowerLeft.Y})");
    }

    [Fact]
    public void Project_WithLateralOffset_ShouldShiftXCoordinates()
    {
        // Arrange
        _mockCar.Position = _mockCar.Position with { LateralOffset = 5.0f }; // Auto a la derecha

        // Act
        var result = _camera.Project(_mockTrack, _mockCar, 800, 600, 10, 0.5f);
        var nearSegment = result.Last();

        // Con el auto a la derecha, la pista debería parecer desplazada a la izquierda en pantalla
        // Centro es 400.
        Assert.True(nearSegment.LowerLeft.X < 400 && nearSegment.LowerRight.X < 800);
    }
}
