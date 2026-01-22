using AutoMapper;
using InsightMed.API.Models;
using InsightMed.Application.Modules.Patients.Models;

namespace InsightMed.API.Mapping;

public sealed class AddPatientProfile : Profile
{
    public AddPatientProfile() => CreateMap<AddPatientInputModel, AddPatientCommandInput>();
}