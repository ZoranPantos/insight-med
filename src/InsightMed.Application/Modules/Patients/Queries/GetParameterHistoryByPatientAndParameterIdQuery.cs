using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Application.Modules.Patients.Models;
using MediatR;
using System.Text.Json;

namespace InsightMed.Application.Modules.Patients.Queries;

public sealed record GetParameterHistoryByPatientAndParameterIdQuery(int PatientId, int ParameterId)
    : IRequest<GetParameterHistoryByPatientAndParameterIdQueryResponse>;

public sealed class GetParameterHistoryByPatientAndParameterIdQueryHandler
    : IRequestHandler<GetParameterHistoryByPatientAndParameterIdQuery, GetParameterHistoryByPatientAndParameterIdQueryResponse>
{
    private readonly ILabReportsService _labReportsService;

    public GetParameterHistoryByPatientAndParameterIdQueryHandler(ILabReportsService labReportsService) =>
        _labReportsService = labReportsService ?? throw new ArgumentNullException(nameof(labReportsService));

    public async Task<GetParameterHistoryByPatientAndParameterIdQueryResponse> Handle(
        GetParameterHistoryByPatientAndParameterIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetParameterHistoryByPatientAndParameterIdQueryResponse();
        bool initialDataAssigned = false;

        var labReportsPatient = await _labReportsService.GetAllByPatientIdAsync(request.PatientId);

        var filteredLabReportsPatient = labReportsPatient
            .Where(labReport => labReport.Content.Contains($"\"Id\":{request.ParameterId}"));
        
        foreach (var labReport in filteredLabReportsPatient)
        {
            var content = JsonSerializer.Deserialize<LabReportContentDto>(labReport.Content)
                ?? throw new Exception($"Deserialized Content of the lab report with ID {labReport.Id} was null");

            var contentItem = content.FirstOrDefault(c => c.Id == request.ParameterId);

            if (contentItem is null) continue;

            if (!initialDataAssigned)
            {
                response.Id = contentItem.Id;
                response.Name = contentItem.Name;
                response.Unit = contentItem.Reference.Unit ?? string.Empty;

                response.LabParameterReference = new()
                {
                    MinThreshold = contentItem.Reference.MinThreshold,
                    MaxThreshold = contentItem.Reference.MaxThreshold,
                    Positive = contentItem.Reference.Positive
                };

                initialDataAssigned = true;
            }

            var historyRecord = new LabParameterHistoryRecordResponse
            {
                Measurement = contentItem?.Measurement,
                IsPositive = contentItem?.IsPositive,
                Created = labReport.Created
            };

            response.History.Add(historyRecord);
        }

        response.History = [.. response.History.OrderBy(x => x.Created)];

        return response;
    }
}