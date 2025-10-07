using AutoMapper;
using InsightMed.Application.LabParameters.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.LabParameters.Mapping;

public sealed class LabParameterMappingProfile : Profile
{
    public LabParameterMappingProfile() => CreateMap<LabParameter, LabParameterResponse>();
}
