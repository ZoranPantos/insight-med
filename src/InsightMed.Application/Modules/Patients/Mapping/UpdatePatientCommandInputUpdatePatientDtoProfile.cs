using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;

namespace InsightMed.Application.Modules.Patients.Mapping;

public sealed class UpdatePatientCommandInputUpdatePatientDtoProfile : Profile
{
    public UpdatePatientCommandInputUpdatePatientDtoProfile() => CreateMap<UpdatePatientCommandInput, UpdatePatientDto>();
}