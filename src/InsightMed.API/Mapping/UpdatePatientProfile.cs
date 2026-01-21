using AutoMapper;
using InsightMed.API.Models;
using InsightMed.Application.Modules.Patients.Models;

namespace InsightMed.API.Mapping;

public sealed class UpdatePatientProfile : Profile
{
    public UpdatePatientProfile() => CreateMap<UpdatePatientInputModel, UpdatePatientCommandInput>();
}