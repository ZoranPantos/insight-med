using AutoMapper;
using InsightMed.Application.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace InsightMed.Infrastructure.Auth.Mapping;

public sealed class IdentityUserMappingProfile : Profile
{
    public IdentityUserMappingProfile() => CreateMap<IdentityUser, IdentityUserResponse>();
}