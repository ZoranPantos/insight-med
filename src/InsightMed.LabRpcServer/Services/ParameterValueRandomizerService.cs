using InsightMed.LabRpcServer.Models;
using InsightMed.LabRpcServer.Services.Abstractions;

namespace InsightMed.LabRpcServer.Services;

internal sealed class ParameterValueRandomizerService : IParameterValueRandomizerService
{
    public RandomizerResult Randomize(LabParameterReference labParameterReference)
    {
        if (!labParameterReference.IsModelValid())
            throw new InvalidOperationException($"Model {nameof(labParameterReference)} is not valid");

        var random = new Random();

        if (labParameterReference.Positive is not null)
        {
            bool isPositive = random.Next() % 2 == 0;
            return new(null, isPositive);
        }

        double? measurement =
            labParameterReference.MinThreshold +
            (labParameterReference.MaxThreshold - labParameterReference.MinThreshold) *
            random.NextDouble();

        if (measurement.HasValue)
            measurement = Math.Round(measurement.Value, 2);

        return new(measurement, null);
    }
}
