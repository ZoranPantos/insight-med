using AutoMapper;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Domain.Entities;
using MediatR;

namespace InsightMed.Application.Modules.Patients.Commands;

public sealed record AddPatientCommand(AddPatientCommandInput Input) : IRequest;

public sealed class AddPatientCommandHandler : IRequestHandler<AddPatientCommand>
{
    private readonly IMapper _mapper;
    private readonly IPatientsService _patientsService;

    public AddPatientCommandHandler(IMapper mapper, IPatientsService patientsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _patientsService = patientsService ?? throw new ArgumentNullException(nameof(patientsService));
    }

    public async Task Handle(AddPatientCommand request, CancellationToken cancellationToken)
    {
        var patient = _mapper.Map<Patient>(request.Input);
        await _patientsService.AddAsync(patient);
    }
}