using AutoMapper;
using InsightMed.Application.Patients.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Patients.Mapping;

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
