using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Queries;
using InsightMed.Application.Modules.LabReports.Validation;
using InsightMed.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class GetAllLabReportsByPatientIdQueryValidationTests
{
    private readonly Mock<IAppDbContext> _mockContext;
    private readonly Mock<DbSet<Patient>> _patients;
    private readonly GetAllLabReportsByPatientIdQueryValidator _validator;
    private readonly List<IValidator<GetAllLabReportsByPatientIdQuery>> _validators;

    private readonly ValidationBehavior<GetAllLabReportsByPatientIdQuery, GetAllLabReportsQueryResponse> _sut;

    public GetAllLabReportsByPatientIdQueryValidationTests()
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

    [Fact]
    public async Task Handle_ShouldThrowInvalidClientDataException_WhenPatientDoesNotExist()
    {
        // Arrange
        int nonExistentPatientId = 99;
        var query = new GetAllLabReportsByPatientIdQuery(nonExistentPatientId);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetAllLabReportsQueryResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(query, nextDelegate.Object, CancellationToken.None));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_ShouldThrow_WhenPatientIdIsZeroOrNegative(int invalidId)
    {
        // Arrange
        var query = new GetAllLabReportsByPatientIdQuery(invalidId);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetAllLabReportsQueryResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(query, nextDelegate.Object, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenPatientExists()
    {
        // Arrange
        int existingId = 1;
        var query = new GetAllLabReportsByPatientIdQuery(existingId);

        var patientList = new List<Patient> { new() { Id = existingId } };
        var mockDbSet = patientList.BuildMockDbSet();
        _mockContext.Setup(x => x.Patients).Returns(mockDbSet.Object);

        var nextDelegate = new Mock<RequestHandlerDelegate<GetAllLabReportsQueryResponse>>();
        nextDelegate.Setup(x => x()).ReturnsAsync(new GetAllLabReportsQueryResponse());

        // Act
        await _sut.Handle(query, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }
}