using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Patients.Mapping
{
    public sealed class AddPatientCommandInputPatientProfile : Profile
    {
        public AddPatientCommandInputPatientProfile()
        {
            CreateMap<AddPatientCommandInput, Patient>()
                .ForMember(
                    dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.DateOfBirth.ToDateTime(TimeOnly.MinValue)));
        }
    }
}