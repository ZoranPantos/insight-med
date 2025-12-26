using AutoMapper;
using InsightMed.Application.Modules.LabRequests.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.LabRequests.Mapping;

public sealed class LabRequestMappingProfile : Profile
{
    public LabRequestMappingProfile()
    {
        CreateMap<LabRequest, LabRequestLiteResponse>()
            .ForMember(
                dest => dest.PatientFullName,
                opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.LastName}")
            )
            .ForMember(
                dest => dest.PatientUid,
                opt => opt.MapFrom(src => src.Patient.Uid)
            );

        CreateMap<LabParameter, LabRequestLabParameterLiteResponse>();
    }
}
