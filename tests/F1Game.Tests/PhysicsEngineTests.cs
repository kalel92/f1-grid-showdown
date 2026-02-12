using Xunit;
using F1Game.Core.Models;
using F1Game.Core.Services;

namespace F1Game.Tests;

public class PhysicsEngineTests
{
    private readonly PhysicsEngine _engine;
    private readonly Car _car;

    public PhysicsEngineTests()
    {
        _engine = new PhysicsEngine();
        _car = new Car
        {
            MaxSpeed = 100.0f,
            BaseAcceleration = 10.0f
        };
    }

    [Fact]
    public void Update_WhenGasIsApplied_SpeedIncreases()
    {
        // Arrange
        float initialSpeed = _car.CurrentSpeed;
        float deltaTime = 1.0f;
        float inputGas = 1.0f;

        // Act
        _engine.Update(_car, deltaTime, inputGas, 0, 0);

        // Assert
        Assert.True(_car.CurrentSpeed > initialSpeed);
    }

    [Fact]
    public void Update_WhenBraking_SpeedDecreases()
    {
        // Arrange
        _car.CurrentSpeed = 50.0f;
        float deltaTime = 1.0f;
        float inputBrake = 1.0f;

        // Act
        _engine.Update(_car, deltaTime, 0, 0, inputBrake);

        // Assert
        Assert.True(_car.CurrentSpeed < 50.0f);
    }

    [Fact]
    public void Update_ShouldNotExceedMaxSpeed()
    {
        // Arrange
        _car.CurrentSpeed = 99.0f;
        float deltaTime = 10.0f; // Suficiente para pasar el límite si no hubiera control
        float inputGas = 1.0f;

        // Act
        _engine.Update(_car, deltaTime, inputGas, 0, 0);

        // Assert
        Assert.Equal(_car.MaxSpeed, _car.CurrentSpeed);
    }

    [Fact]
    public void Update_WhenNoInput_SpeedDecreasesDueToFriction()
    {
        // Arrange
        _car.CurrentSpeed = 20.0f;
        float deltaTime = 1.0f;

        // Act
        _engine.Update(_car, deltaTime, 0, 0, 0);

        // Assert
        Assert.True(_car.CurrentSpeed < 20.0f);
    }
}
