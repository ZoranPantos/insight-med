namespace InsightMed.API.Models;

public sealed record AddPatientInputModel(
    string FirstName,
    string LastName,
    string Uid,
    string Phone,
    string Email,
    DateOnly DateOfBirth,
    int Gender,
    int BloodGroup,
    int SmokingStatus,
    int ExerciseLevel,
    int DietType,
    double HeightCm,
    double WeightKg);