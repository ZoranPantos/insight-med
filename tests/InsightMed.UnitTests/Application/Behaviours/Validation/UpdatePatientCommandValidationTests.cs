using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Commands;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Validation;
using InsightMed.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class UpdatePatientCommandValidationTests
{
    private readonly Mock<IAppDbContext> _mockContext;
    private readonly UpdatePatientCommandValidator _validator;
    private readonly List<IValidator<UpdatePatientCommand>> _validators;
    private readonly Mock<DbSet<Patient>> _patients;

    private readonly ValidationBehavior<UpdatePatientCommand, Unit> _sut;

    public UpdatePatientCommandValidationTests()
    {
        _mockContext = new();

        _patients = new List<Patient>().BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(_patients.Object);

        _validator = new(_mockContext.Object);
        _validators = [_validator];

        _sut = new(_validators);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(300)]
    [InlineData(-5)]
    public async Task Handle_ShouldThrow_WhenHeightIsInvalid(double invalidHeight)
    {
        // Arrange
        var input = CreateValidInput();
        input.HeightCm = invalidHeight;
        var command = new UpdatePatientCommand(input);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains("Height value is invalid", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientDoesNotExist()
    {
        // Arrange
        int nonExistentId = 999;

        var input = CreateValidInput();
        input.Id = nonExistentId;

        var command = new UpdatePatientCommand(input);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains($"Patient with ID {nonExistentId} not found", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenCommandIsValid()
    {
        // Arrange
        int existingId = 1;
        var input = CreateValidInput();
        input.Id = existingId;
        var command = new UpdatePatientCommand(input);

        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();
        nextDelegate.Setup(x => x()).ReturnsAsync(Unit.Value);

        var patients = new List<Patient> { new() { Id = existingId } }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(patients.Object);

        // Act
        await _sut.Handle(command, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }

    private UpdatePatientCommandInput CreateValidInput() => new()
    {
        Id = 1,
        HeightCm = 170,
        WeightKg = 70
    };
}