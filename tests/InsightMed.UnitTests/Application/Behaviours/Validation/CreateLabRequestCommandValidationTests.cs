using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabRequests.Commands;
using InsightMed.Application.Modules.LabRequests.Validation;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class CreateLabRequestCommandValidationTests
{
    private readonly Mock<IAppDbContext> _mockContext;
    private readonly CreateLabRequestCommandValidator _validator;
    private readonly List<IValidator<CreateLabRequestCommand>> _validators;
    private readonly Mock<DbSet<Patient>> _patients;
    private readonly Mock<DbSet<LabRequest>> _labRequests;

    private readonly ValidationBehavior<CreateLabRequestCommand, Unit> _sut;

    public CreateLabRequestCommandValidationTests()
    {
        _mockContext = new();

        _patients = new List<Patient>().BuildMockDbSet();
        _labRequests = new List<LabRequest>().BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(_patients.Object);

        _mockContext
            .Setup(x => x.LabRequests)
            .Returns(_labRequests.Object);

        _validator = new(_mockContext.Object);
        _validators = [_validator];

        _sut = new ValidationBehavior<CreateLabRequestCommand, Unit>(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientDoesNotExist()
    {
        // Arrange
        var command = new CreateLabRequestCommand(99, [1, 2]);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains("Patient with ID 99 not found", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenLabParameterIdsContainDuplicates()
    {
        // Arrange
        var command = new CreateLabRequestCommand(1, [1, 2, 2]);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        var patients = new List<Patient> { new() { Id = 1 } }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(patients.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains("LabParameterIds must not contain duplicate values", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDuplicatePendingRequestExists()
    {
        // Arrange
        int patientId = 1;
        var parameterIds = new List<int> { 101, 102 };
        var command = new CreateLabRequestCommand(patientId, parameterIds);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        var patients = new List<Patient> { new() { Id = patientId } }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(patients.Object);

        var existingRequests = new List<LabRequest>
        {
            new()
            {
                PatientId = patientId,
                LabRequestState = LabRequestState.Pending,
                LabParameterIds = [101, 102]
            }
        }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.LabRequests)
            .Returns(existingRequests.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains("A pending lab request with the same patient and identical lab parameters already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenCommandIsValid()
    {
        // Arrange
        int patientId = 1;
        var command = new CreateLabRequestCommand(patientId, [101, 102]);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        var patients = new List<Patient> { new() { Id = patientId } }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(patients.Object);

        var labRequests = new List<LabRequest>().BuildMockDbSet();

        _mockContext
            .Setup(x => x.LabRequests)
            .Returns(labRequests.Object);

        // Act
        await _sut.Handle(command, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }
}