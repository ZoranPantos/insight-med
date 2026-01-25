using InsightMed.Application.Modules.Notifications.Enums;
using InsightMed.Application.Modules.Notifications.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace InsightMed.IntegrationTests.Endpoints;

public sealed class NotificationsTests : BaseIntegrationTest, IAsyncLifetime
{
    public NotificationsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    public async ValueTask InitializeAsync()
    {
        await SeedAsync();
        await AuthenticateAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetEndpoint_ShouldRetrieveExistingNotifications()
    {
        // Act
        var response = await client.GetAsync("api/notifications", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var notificationsData = await response.Content
            .ReadFromJsonAsync<GetAllNotificationsQueryResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(notificationsData);
        Assert.NotNull(notificationsData.Notifications);
        Assert.NotEmpty(notificationsData.Notifications);
    }

    [Fact]
    public async Task SeenEndpoint_ShouldMarkUnseenNotificationsAsSeen()
    {
        // Arrange
        var query = new Dictionary<string, string?>
        {
            ["filter"] = NotificationFilter.Unseen.ToString()
        };

        var url = QueryHelpers.AddQueryString("api/notifications", query);

        var unseenNotificationsResponse = await client.GetAsync(url, TestContext.Current.CancellationToken);
        unseenNotificationsResponse.EnsureSuccessStatusCode();

        var unseenNotificationData = await unseenNotificationsResponse.Content
            .ReadFromJsonAsync<GetAllNotificationsQueryResponse>(TestContext.Current.CancellationToken);

        Assert.NotNull(unseenNotificationData);
        Assert.NotNull(unseenNotificationData.Notifications);

        var unseenNotifications = unseenNotificationData.Notifications;

        var unseenNotificationIds = unseenNotifications?
            .Select(notification => notification.Id)
            .ToList();

        var statusesBeforeChange = await context.Notifications
            .Where(notification => unseenNotificationIds!.Contains(notification.Id))
            .ToDictionaryAsync(
                notification => notification.Id,
                notification => notification.Seen,
                TestContext.Current.CancellationToken);

        // Act
        context.ChangeTracker.Clear();

        var response = await client.PutAsJsonAsync(
            "api/notifications/seen", unseenNotificationIds, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var statusesAfterChange = await context.Notifications
            .Where(notification => unseenNotificationIds!.Contains(notification.Id))
            .ToDictionaryAsync(
                notification => notification.Id,
                notification => notification.Seen,
                TestContext.Current.CancellationToken);

        // Assert
        Assert.All(statusesBeforeChange, item =>
        {
            Assert.False(item.Value);
        });

        Assert.All(statusesAfterChange, item =>
        {
            Assert.True(item.Value);
        });
    }
}