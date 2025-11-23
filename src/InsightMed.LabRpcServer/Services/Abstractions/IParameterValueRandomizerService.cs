using InsightMed.LabRpcServer.Models;

namespace InsightMed.LabRpcServer.Services.Abstractions;

internal interface IParameterValueRandomizerService
{
    RandomizerResult Randomize(LabParameterReference labParameterReference);
}
