using FluentValidation;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Notifications.Commands;
using InsightMed.Application.Modules.Notifications.Validation;
using InsightMed.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace InsightMed.UnitTests.Application.Behaviours.Validation;

public sealed class CreateNotificationCommandValidationTests
{
    private readonly Mock<IAppDbContext> _mockContext;
    private readonly CreateNotificationCommandValidator _validator;
    private readonly List<IValidator<CreateNotificationCommand>> _validators;
    private readonly Mock<DbSet<LabReport>> _labReports;

    private readonly ValidationBehavior<CreateNotificationCommand, Unit> _sut;

    public CreateNotificationCommandValidationTests()
    {
        _mockContext = new();
        _labReports = new List<LabReport>().BuildMockDbSet();

        _mockContext
            .Setup(x => x.LabReports)
            .Returns(_labReports.Object);

        _validator = new(_mockContext.Object);
        _validators = [_validator];

        _sut = new(_validators);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidClientDataException_WhenMessageIsEmpty()
    {
        // Arrange
        var command = new CreateNotificationCommand(LabReportId: 1, Message: string.Empty);
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains("'Message' must not be empty", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidClientDataException_WhenLabReportDoesNotExist()
    {
        // Arrange
        int nonExistentId = 99;
        var command = new CreateNotificationCommand(LabReportId: nonExistentId, Message: "Test");
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidClientDataException>(() =>
            _sut.Handle(command, nextDelegate.Object, CancellationToken.None));

        Assert.Contains($"Lab Report with ID {nonExistentId} not found", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenCommandIsValid()
    {
        // Arrange
        int existingId = 1;
        var command = new CreateNotificationCommand(LabReportId: existingId, Message: "Valid Message");
        var nextDelegate = new Mock<RequestHandlerDelegate<Unit>>();
        nextDelegate.Setup(x => x()).ReturnsAsync(Unit.Value);

        var labReports = new List<LabReport> { new() { Id = existingId } }.BuildMockDbSet();

        _mockContext
            .Setup(x => x.LabReports)
            .Returns(labReports.Object);

        // Act
        await _sut.Handle(command, nextDelegate.Object, CancellationToken.None);

        // Assert
        nextDelegate.Verify(x => x(), Times.Once);
    }
}