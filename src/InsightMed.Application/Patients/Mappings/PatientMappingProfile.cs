using AutoMapper;
using InsightMed.Application.Patients.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Patients.Mappings;

public sealed class PatientMappingProfile : Profile
{
    public PatientMappingProfile() => CreateMap<Patient, PatientLiteResponse>();
}
