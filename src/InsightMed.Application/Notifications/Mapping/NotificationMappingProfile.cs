using AutoMapper;
using InsightMed.Application.Notifications.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Notifications.Mapping;

public sealed class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile() => CreateMap<Notification, NotificationLiteResponse>();
}
