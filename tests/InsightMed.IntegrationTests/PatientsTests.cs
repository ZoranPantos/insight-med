using InsightMed.API.Models;
using System.Text;
using System.Text.Json;

namespace InsightMed.IntegrationTests;

public class PatientsTests : BaseIntegrationTest
{
    public PatientsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AddEndpoint_ShouldSavePatientToDatabase()
    {
        // Arrange
        await SeedAsync();
        await AuthenticateAsync();

        var newPatient = new AddPatientInputModel(
            FirstName: "Test",
            LastName: "Test",
            Uid: "UID-0000",
            Phone: "+000000",
            Email: "test@test.com",
            DateOfBirth: new DateOnly(1985, 5, 20),
            Gender: 1,
            BloodGroup: 1,
            SmokingStatus: 0,
            ExerciseLevel: 1,
            DietType: 0,
            HeightCm: 180.5,
            WeightKg: 75.0);

        // Act
        string jsonData = JsonSerializer.Serialize(newPatient);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/patients", content, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        // Assert
        var addedPatient = context.Patients.FirstOrDefault(
            patient => patient.FirstName.Equals(newPatient.FirstName) &&
            patient.LastName.Equals(newPatient.LastName) &&
            patient.Uid.Equals(newPatient.Uid) &&
            patient.Phone.Equals(newPatient.Phone) &&
            patient.Email.Equals(newPatient.Email));

        Assert.NotNull(addedPatient);
    }
}