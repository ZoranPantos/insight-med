using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.LabParameters.Mapping;

public sealed class LabParameterMappingProfile : Profile
{
    public LabParameterMappingProfile() => CreateMap<LabParameter, LabParameterResponse>();
}
