using AutoMapper;
using InsightMed.Application.Auth.Models;

namespace InsightMed.Application.Auth.Mapping;

public sealed class IdentityUserResponseMappingProfile : Profile
{
    public IdentityUserResponseMappingProfile() => CreateMap<IdentityUserResponse, GetAccountInfoQueryResponse>();
}