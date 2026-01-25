using InsightMed.LabRpcServer.Models;
using InsightMed.LabRpcServer.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace InsightMed.UnitTests.LabRpcServer.Services;

public sealed class ParameterValueRandomizerServiceTests
{
    private readonly Mock<ILogger<ParameterValueRandomizerService>> _mockLogger;
    private readonly ParameterValueRandomizerService _sut;

    public ParameterValueRandomizerServiceTests()
    {
        _mockLogger = new();
        _sut = new(_mockLogger.Object);
    }

    [Fact]
    public void Randomize_ShouldThrowInvalidOperationException_WhenModelIsInvalid()
    {
        // Arrange
        var invalidRef = new LabParameterReference
        {
            MinThreshold = 10,
            MaxThreshold = 20,
            Positive = true
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.Randomize(invalidRef));

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("is not valid")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Randomize_ShouldReturnBooleanResult_WhenPositiveIsProvided()
    {
        // Arrange
        var labRef = new LabParameterReference { Positive = false };

        // Act
        var result = _sut.Randomize(labRef);

        // Assert
        Assert.Null(result.Value);
        Assert.NotNull(result.IsPositive);
        Assert.IsType<bool>(result.IsPositive.Value);
    }

    [Theory]
    [InlineData(10.0, 20.0)]
    [InlineData(100.0, 150.0)]
    public void Randomize_ShouldReturnNumericalResultWithinExtendedRange_WhenThresholdsAreProvided(double min, double max)
    {
        // Arrange
        var labRef = new LabParameterReference
        {
            MinThreshold = min,
            MaxThreshold = max,
            Unit = "mg/dL"
        };

        double extension = (max - min) / 2;
        double absoluteMin = Math.Max(0, min - extension);
        double absoluteMax = max + extension;

        // Act
        var result = _sut.Randomize(labRef);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Null(result.IsPositive);
        Assert.InRange(result.Value.Value, absoluteMin, absoluteMax);
    }

    [Fact]
    public void Randomize_ShouldNotReturnNegativeValues_WhenRangeExtensionGoesBelowZero()
    {
        // Arrange
        var labRef = new LabParameterReference
        {
            MinThreshold = 1.0,
            MaxThreshold = 10.0,
            Unit = "g/L"
        };

        // Act & Assert
        for (int i = 0; i < 50; i++)
        {
            var result = _sut.Randomize(labRef);
            Assert.True(result.Value >= 0, $"Value {result.Value} should not be negative");
        }
    }
}