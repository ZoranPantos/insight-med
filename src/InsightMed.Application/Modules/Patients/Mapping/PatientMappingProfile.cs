using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Patients.Mapping;

public sealed class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<Patient, PatientLiteResponse>();
        CreateMap<Patient, GetPatientByIdQueryResponse>();
        CreateMap<LabReport, PatientLabReportResponse>();
        CreateMap<LabRequest, PatientLabRequestResponse>();
    }
}
