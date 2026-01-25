using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Queries;
using InsightMed.Application.Modules.Patients.Validation;
using InsightMed.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class GetParameterHistoryByPatientAndParameterIdQueryValidationTests
{
    private readonly Mock<IAppDbContext> _mockContext;
    private readonly GetParameterHistoryByPatientAndParameterIdQueryValidator _validator;
    private readonly List<IValidator<GetParameterHistoryByPatientAndParameterIdQuery>> _validators;
    private readonly Mock<DbSet<Patient>> _patients;
    private readonly Mock<DbSet<LabParameter>> _labParameters;

    private readonly ValidationBehavior<GetParameterHistoryByPatientAndParameterIdQuery, GetParameterHistoryByPatientAndParameterIdQueryResponse> _sut;

    public GetParameterHistoryByPatientAndParameterIdQueryValidationTests()
    {
        _mockContext = new();

        _patients = new List<Patient>().BuildMockDbSet();
        _labParameters = new List<LabParameter>().BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(_patients.Object);

        _mockContext
            .Setup(x => x.LabParameters)
            .Returns(_labParameters.Object);

        _validator = new(_mockContext.Object);
        _validators = [_validator];

        _sut = new(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientDoesNotExist()
    {
        // Arrange
        int nonExistentPatientId = 99;
        var query = new GetParameterHistoryByPatientAndParameterIdQuery(nonExistentPatientId, 1);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetParameterHistoryByPatientAndParameterIdQueryResponse>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(query, nextDelegate.Object, CancellationToken.None));

        Assert.Contains($"Patient with ID {nonExistentPatientId} not found", exception.Message);
        nextDelegate.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenLabParameterDoesNotExist()
    {
        // Arrange
        int patientId = 1;
        int nonExistentParamId = 500;
        var query = new GetParameterHistoryByPatientAndParameterIdQuery(patientId, nonExistentParamId);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetParameterHistoryByPatientAndParameterIdQueryResponse>>();

        var patients = new List<Patient> { new() { Id = patientId } }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(patients.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(query, nextDelegate.Object, CancellationToken.None));

        Assert.Contains($"Lab parameter with ID {nonExistentParamId} not found", exception.Message);
        nextDelegate.Verify(x => x(), Times.Never);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, -1)]
    public async Task Handle_ShouldThrow_WhenIdsAreZeroOrNegative(int patientId, int paramId)
    {
        // Arrange
        var query = new GetParameterHistoryByPatientAndParameterIdQuery(patientId, paramId);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetParameterHistoryByPatientAndParameterIdQueryResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(query, nextDelegate.Object, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenIdsAreValidAndExist()
    {
        // Arrange
        int patientId = 1;
        int paramId = 1;
        var query = new GetParameterHistoryByPatientAndParameterIdQuery(patientId, paramId);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetParameterHistoryByPatientAndParameterIdQueryResponse>>();

        nextDelegate
            .Setup(x => x())
            .ReturnsAsync(new GetParameterHistoryByPatientAndParameterIdQueryResponse());

        var patients = new List<Patient> { new() { Id = patientId } }.BuildMockDbSet();
        var labParameters = new List<LabParameter> { new() { Id = paramId } }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.Patients)
            .Returns(patients.Object);

        _mockContext
            .Setup(x => x.LabParameters)
            .Returns(labParameters.Object);

        // Act
        await _sut.Handle(query, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }
}