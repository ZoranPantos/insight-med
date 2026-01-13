using InsightMed.Application.Common.Models;

namespace InsightMed.Application.Modules.Patients.Models;

public sealed class GetAllPatientsQueryResponse : BasePagedResponse
{
    public List<PatientLiteResponse> Patients { get; set; } = [];
}