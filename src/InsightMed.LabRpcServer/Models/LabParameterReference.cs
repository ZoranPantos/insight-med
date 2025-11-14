namespace InsightMed.LabRpcServer.Models;

internal sealed class LabParameterReference
{
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public bool? Positive { get; set; }

    public bool IsModelValid()
    {
        bool thresholdCheck =
            MinThreshold is not null &&
            MaxThreshold is not null &&
            Positive is null;

        bool positiveCheck =
            MinThreshold is null &&
            MaxThreshold is null &&
            Positive is not null;

        return thresholdCheck || positiveCheck;
    }
}
