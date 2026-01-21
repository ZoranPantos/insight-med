using AutoMapper;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Modules.Patients.Commands;

public sealed record UpdatePatientCommand(UpdatePatientCommandInput Input) : IRequest;

public sealed class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand>
{
    private readonly IMapper _mapper;
    private readonly IPatientsService _patientsService;

    public UpdatePatientCommandHandler(IMapper mapper, IPatientsService patientsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _patientsService = patientsService ?? throw new ArgumentNullException(nameof(patientsService));
    }

    public async Task Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var updatePatientDto = _mapper.Map<UpdatePatientDto>(request.Input);

        bool result = await _patientsService.UpdateAsync(request.Input.Id, updatePatientDto);

        if (!result)
            throw new ResourceNotFoundException($"Patient with ID {request.Input.Id} not found");
    }
}