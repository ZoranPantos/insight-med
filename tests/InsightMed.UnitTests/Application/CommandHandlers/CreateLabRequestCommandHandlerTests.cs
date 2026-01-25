using InsightMed.Application.Auth.Services.Abstractions;
using InsightMed.Application.Common.Abstractions.Messaging;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabRequests.Commands;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace InsightMed.UnitTests.Application.CommandHandlers;

public sealed class CreateLabRequestCommandHandlerTests
{
    private readonly Mock<ILogger<CreateLabRequestCommandHandler>> _mockLogger;
    private readonly Mock<ILabRequestsService> _mockLabRequestsService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ILabRpcClient> _mockLabRpcClient;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    private readonly CreateLabRequestCommandHandler _sut;

    public CreateLabRequestCommandHandlerTests()
    {
        _mockLogger = new();
        _mockLabRequestsService = new();
        _mockCurrentUserService = new();
        _mockLabRpcClient = new();
        _mockServiceScopeFactory = new();

        _sut = new(
            _mockLogger.Object,
            _mockLabRequestsService.Object,
            _mockCurrentUserService.Object,
            _mockLabRpcClient.Object,
            _mockServiceScopeFactory.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldThrow_ForNonExistentUser(string? userId)
    {
        // Arrange
        var command = new CreateLabRequestCommand(PatientId: 1, LabParameterIds: [1, 2, 3]);

        _mockCurrentUserService
            .Setup(x => x.GetUserId())
            .Returns(userId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _sut.Handle(command, CancellationToken.None));

        _mockLabRequestsService.Verify(
            x => x.AddAsync(It.IsAny<LabRequest>()),
            Times.Never);
    }
}