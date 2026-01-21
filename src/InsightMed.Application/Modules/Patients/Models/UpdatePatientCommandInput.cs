using InsightMed.Domain.Enums;

namespace InsightMed.Application.Modules.Patients.Models;

public sealed class UpdatePatientCommandInput
{
    public int Id { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public SmokingStatus SmokingStatus { get; set; }
    public ExerciseLevel ExerciseLevel { get; set; }
    public DietType DietType { get; set; }
}