using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;

namespace InsightMed.Application.Modules.Patients.Mapping;

public sealed class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<Patient, PatientLiteResponse>();
        CreateMap<Patient, GetPatientByIdQueryResponse>();
        CreateMap<LabReport, PatientLabReportResponse>();

        CreateMap<LabRequest, PatientLabRequestResponse>()
            .ForMember(
                dest => dest.LabReportId,
                opt => opt.MapFrom(src =>
                    (src.LabReport != null && src.LabRequestState == LabRequestState.Completed)
                        ? src.LabReport.Id
                        : (int?)null));
    }
}