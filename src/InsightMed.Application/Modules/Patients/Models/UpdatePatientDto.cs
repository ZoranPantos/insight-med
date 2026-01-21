using InsightMed.Domain.Enums;

namespace InsightMed.Application.Modules.Patients.Models;

public sealed record UpdatePatientDto(
    double HeightCm,
    double WeightKg,
    SmokingStatus SmokingStatus,
    ExerciseLevel ExerciseLevel,
    DietType DietType);