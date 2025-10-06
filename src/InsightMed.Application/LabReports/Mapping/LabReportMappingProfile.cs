using AutoMapper;
using InsightMed.Application.LabReports.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.LabReports.Mapping;

public sealed class LabReportMappingProfile : Profile
{
    public LabReportMappingProfile()
    {
        CreateMap<LabReport, LabReportLiteResponse>()
            .ForMember(
                dest => dest.PatientFullName,
                opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.LastName}"))
            .ForMember(
                dest => dest.PatientUid,
                opt => opt.MapFrom(src => src.Patient.Uid));

        CreateMap<LabReport, GetLabReportByIdQueryResponse>()
            .ForMember(
                dest => dest.PatientFullName,
                opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.LastName}"))
            .ForMember(
                dest => dest.PatientUid,
                opt => opt.MapFrom(src => src.Patient.Uid));
    }
}
