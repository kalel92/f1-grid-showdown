using Xunit;
using System.IO;
using F1Game.Services.Implementations;
using F1Game.Services.Interfaces;

namespace F1Game.Tests;

public class UserDataServiceTests : IDisposable
{
    private readonly string _testPath = "test_saves";

    public UserDataServiceTests()
    {
        if (Directory.Exists(_testPath)) Directory.Delete(_testPath, true);
        Directory.CreateDirectory(_testPath);
    }

    [Fact]
    public void SaveProfile_ShouldPersistNewRecordCorrectly()
    {
        // Arrange
        var service = new UserDataService(_testPath);
        var profile = new UserProfile 
        { 
            Username = "ChecoPerez", 
            BestLapTime = 72.543f, 
            AccumulatedMoney = 1500000m 
        };

        // Act
        service.SaveProfile(profile);
        var loadedProfile = service.GetOrCreateProfile("ChecoPerez");

        // Assert
        Assert.Equal(profile.Username, loadedProfile.Username);
        Assert.Equal(profile.BestLapTime, loadedProfile.BestLapTime);
        Assert.Equal(profile.AccumulatedMoney, loadedProfile.AccumulatedMoney);
    }

    [Fact]
    public void GetOrCreateProfile_ShouldReturnEmpty_WhenUserDoesNotExist()
    {
        // Arrange
        var service = new UserDataService(_testPath);

        // Act
        var profile = service.GetOrCreateProfile("NewRacer");

        // Assert
        Assert.Equal("NewRacer", profile.Username);
        Assert.Equal(float.MaxValue, profile.BestLapTime);
        Assert.Equal(0, profile.AccumulatedMoney);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testPath)) Directory.Delete(_testPath, true);
    }
}
