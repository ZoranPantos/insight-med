using InsightMed.LabRpcServer.Models;
using InsightMed.LabRpcServer.Services.Abstractions;

namespace InsightMed.LabRpcServer.Services;

internal sealed class ParameterValueRandomizerService : IParameterValueRandomizerService
{
    private readonly ILogger<ParameterValueRandomizerService> _logger;

    public ParameterValueRandomizerService(ILogger<ParameterValueRandomizerService> logger) =>
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public RandomizerResult Randomize(LabParameterReference labParameterReference)
    {
        if (!labParameterReference.IsModelValid())
        {
            string message = $"Failed to randomize lab parameters. Model {nameof(labParameterReference)} is not valid";

            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }

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
