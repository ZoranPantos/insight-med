namespace InsightMed.API.Models;

public sealed record UpdatePatientInputModel(
    double HeightCm,
    double WeightKg,
    int SmokingStatus,
    int ExerciseLevel,
    int DietType);