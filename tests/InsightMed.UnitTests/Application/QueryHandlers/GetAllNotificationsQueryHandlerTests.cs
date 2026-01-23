using AutoMapper;
using InsightMed.Application.Modules.Notifications.Enums;
using InsightMed.Application.Modules.Notifications.Mapping;
using InsightMed.Application.Modules.Notifications.Queries;
using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using InsightMed.Domain.Entities;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetAllNotificationsQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<INotificationsService> _mockNotificationsService;

    private readonly GetAllNotificationsQueryHandler _sut;

    public GetAllNotificationsQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<NotificationMappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _mockNotificationsService = new();

        _sut = new GetAllNotificationsQueryHandler(_mapper, _mockNotificationsService.Object);
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectly()
    {
        // Arrange
        string requesterId = "test-guid";
        var filter = NotificationFilter.Unseen;
        var query = new GetAllNotificationsQuery(requesterId, filter);

        var sourceNotifications = new List<Notification>
        {
            new() {
                Id = 101,
                Message = "Lab Report Ready",
                LabReportId = 50,
                Seen = false,
                RequesterId = requesterId
            },
            new() {
                Id = 102,
                Message = "Critical Result",
                LabReportId = 51,
                Seen = false,
                RequesterId = requesterId
            }
        };

        _mockNotificationsService
            .Setup(s => s.GetAllAsync(requesterId, filter))
            .ReturnsAsync(sourceNotifications);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Notifications);
        Assert.Equal(2, result.Notifications.Count);

        var firstItem = result.Notifications[0];
        Assert.Equal(101, firstItem.Id);
        Assert.Equal("Lab Report Ready", firstItem.Message);
        Assert.Equal(50, firstItem.LabReportId);

        var secondItem = result.Notifications[1];
        Assert.Equal(102, secondItem.Id);
        Assert.Equal("Critical Result", secondItem.Message);
        Assert.Equal(51, secondItem.LabReportId);

        _mockNotificationsService.Verify(s => s.GetAllAsync(requesterId, filter), Times.Once);
    }
}