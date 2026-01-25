using InsightMed.API.Models;
using InsightMed.Application.Modules.Patients.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace InsightMed.IntegrationTests.Endpoints;

public sealed class PatientsTests : BaseIntegrationTest, IAsyncLifetime
{
    public PatientsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    public async ValueTask InitializeAsync()
    {
        await SeedAsync();
        await AuthenticateAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetEndpoint_ShouldRetrieveExistingPatient()
    {
        // Arrange
        int patientId = 1;

        // Act
        var response = await client.GetAsync($"api/patients/{patientId}", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var patient = await response.Content
            .ReadFromJsonAsync<GetPatientByIdQueryResponse>(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(patient);
        Assert.Equal(patientId, patient.Id);
    }

    [Fact]
    public async Task AddEndpoint_ShouldSavePatientToDatabase()
    {
        // Arrange
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

    [Fact]
    public async Task UpdateEndpoint_ShouldUpdateExistingPatient()
    {
        // Arrange
        int patientId = 1;

        var updatePatientModel = new UpdatePatientInputModel(
            HeightCm: 1.1,
            WeightKg: 1.1,
            SmokingStatus: 1,
            ExerciseLevel: 0,
            DietType: 1);

        // Act
        string jsonData = JsonSerializer.Serialize(updatePatientModel);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"api/patients/{patientId}", content, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        // Assert
        var updatedPatient = context.Patients.FirstOrDefault(
            patient => patient.Id == patientId &&
            patient.HeightCm == updatePatientModel.HeightCm &&
            patient.WeightKg == updatePatientModel.WeightKg &&
            (int)patient.SmokingStatus == updatePatientModel.SmokingStatus &&
            (int)patient.ExerciseLevel == updatePatientModel.ExerciseLevel &&
            (int)patient.DietType == updatePatientModel.DietType);

        Assert.NotNull(updatedPatient);
    }
}