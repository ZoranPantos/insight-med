using FluentValidation;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Queries;
using InsightMed.Application.Modules.Patients.Validation;
using MediatR;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class GetPatientByIdQueryValidationTests
{
    private readonly GetPatientByIdQueryValidator _validator;
    private readonly List<IValidator<GetPatientByIdQuery>> _validators;

    private readonly ValidationBehavior<GetPatientByIdQuery, GetPatientByIdQueryResponse> _sut;

    public GetPatientByIdQueryValidationTests()
    {
        _validator = new();
        _validators = [_validator];

        _sut = new(_validators);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handle_ShouldThrowInvalidClientDataException_WhenIdIsZeroOrNegative(int invalidId)
    {
        // Arrange
        var query = new GetPatientByIdQuery(invalidId, 1);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetPatientByIdQueryResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(query, nextDelegate.Object, CancellationToken.None));

        nextDelegate.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenIdIsValid()
    {
        // Arrange
        var query = new GetPatientByIdQuery(1, 1);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetPatientByIdQueryResponse>>();

        nextDelegate
            .Setup(x => x())
            .ReturnsAsync(new GetPatientByIdQueryResponse());

        // Act
        await _sut.Handle(query, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }
}