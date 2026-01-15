namespace InsightMed.LabRpcServer.Models;

internal sealed class LabResponseDto
{
    public List<LabParameterValueResponseDto> LabParameterValueResponseDtos { get; set; } = [];
}