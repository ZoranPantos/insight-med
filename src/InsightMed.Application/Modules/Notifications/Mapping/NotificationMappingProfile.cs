using AutoMapper;
using InsightMed.Application.Modules.Notifications.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Notifications.Mapping;

public sealed class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile() => CreateMap<Notification, NotificationLiteResponse>();
}