using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Commands;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Validation;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class AddPatientCommandValidationTests
{
    private readonly Mock<IAppDbContext> _mockContext;
    private readonly AddPatientCommandValidator _validator;
    private readonly List<IValidator<AddPatientCommand>> _validators;
    private readonly Mock<DbSet<Patient>> _patients;
    private readonly ValidationBehavior<AddPatientCommand, Unit> _sut;

    public AddPatientCommandValidationTests()
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
    [InlineData(-10)]
    public async Task Handle_ShouldThrow_WhenHeightIsInvalid(double invalidHeight)
    {
        // Arrange
        var input = CreateValidInput();
        input.HeightCm = invalidHeight;

        var command = new AddPatientCommand(input);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains("Height value is invalid", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDateOfBirthIsInFuture()
    {
        // Arrange
        var input = CreateValidInput();
        input.DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var command = new AddPatientCommand(input);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains("Date of birth cannot be in the future", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUidAlreadyExists()
    {
        // Arrange
        string existingUid = "UID-123";
        var input = CreateValidInput();
        input.Uid = existingUid;

        var command = new AddPatientCommand(input);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        var patients = new List<Patient> { new() { Uid = existingUid } }.BuildMockDbSet();
        _mockContext.Setup(x => x.Patients).Returns(patients.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains($"Patient with UID {existingUid} already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenCommandIsValid()
    {
        // Arrange
        var input = CreateValidInput();
        var command = new AddPatientCommand(input);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();
        nextDelegate.Setup(x => x()).ReturnsAsync(Unit.Value);

        // Act
        await _sut.Handle(command, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }

    private AddPatientCommandInput CreateValidInput() => new()
    {
        FirstName = "John",
        LastName = "Doe",
        Uid = "UNIQUE-001",
        Phone = "123456789",
        Email = "john@doe.com",
        DateOfBirth = new DateOnly(1990, 1, 1),
        Gender = (Gender)1,
        BloodGroup = (BloodGroup)1,
        SmokingStatus = 0,
        ExerciseLevel = (ExerciseLevel)1,
        DietType = (DietType)0,
        HeightCm = 175,
        WeightKg = 70
    };
}