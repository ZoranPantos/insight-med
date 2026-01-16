using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Patients.Mapping;

public sealed class LabParameterEvaluatedLabParameterMappingProfile : Profile
{
    public LabParameterEvaluatedLabParameterMappingProfile() =>
        CreateMap<LabParameter, EvaluatedLabParameterResponse>();
}