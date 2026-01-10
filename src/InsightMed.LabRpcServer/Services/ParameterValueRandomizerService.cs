using InsightMed.LabRpcServer.Models;
using InsightMed.LabRpcServer.Services.Abstractions;

namespace InsightMed.LabRpcServer.Services;

/// <summary>
/// Lab parameter can as a normal measuerd value be bool or have a numerical range.
/// If it is a bool, it can for example be false as normal - which means true would raise concern (e.g. tumor marker).
/// If it has a numerical normal range, then values outside of that range would raise concern (e.g. hemoglobin less than 12.0).
/// This service generates a random true/false value or a value from a given normal range +/- some extension to simulate bad results.
/// </summary>
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

        double rangeExtension = (labParameterReference.MaxThreshold!.Value - labParameterReference.MinThreshold!.Value) / 2;

        double min = labParameterReference.MinThreshold!.Value - rangeExtension;
        double max = labParameterReference.MaxThreshold!.Value + rangeExtension;

        if (min < 0) min = 0;

        double? measurement = min + (max - min) * random.NextDouble();

        if (measurement.HasValue)
            measurement = Math.Round(measurement.Value, 2);

        return new(measurement, null);
    }
}