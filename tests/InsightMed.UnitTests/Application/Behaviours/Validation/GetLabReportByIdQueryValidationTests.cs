using FluentValidation;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Queries;
using InsightMed.Application.Modules.LabReports.Validation;
using MediatR;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class GetLabReportByIdQueryValidationTests
{
    private readonly GetLabReportByIdQueryValidator _validator;
    private readonly List<IValidator<GetLabReportByIdQuery>> _validators;

    private readonly ValidationBehavior<GetLabReportByIdQuery, GetLabReportByIdQueryResponse> _sut;

    public GetLabReportByIdQueryValidationTests()
    {
        _validator = new();
        _validators = [_validator];

        _sut = new(_validators);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99)]
    public async Task Handle_ShouldThrowInvalidClientDataException_WhenIdIsZeroOrNegative(int invalidId)
    {
        // Arrange
        var query = new GetLabReportByIdQuery(invalidId);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetLabReportByIdQueryResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(query, nextDelegate.Object, CancellationToken.None));

        nextDelegate.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenIdIsValid()
    {
        // Arrange
        var query = new GetLabReportByIdQuery(1);
        var nextDelegate = new Mock<RequestHandlerDelegate<GetLabReportByIdQueryResponse>>();

        nextDelegate.Setup(x => x()).ReturnsAsync(new GetLabReportByIdQueryResponse());

        // Act
        await _sut.Handle(query, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }
}