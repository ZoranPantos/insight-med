using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Modules.Patients.Mapping;
using InsightMed.Application.Modules.Patients.Queries;
using InsightMed.Domain.Entities;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetEvaluatedParametersByPatientIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILabParametersService> _mockLabParametersService;

    private readonly GetEvaluatedParametersByPatientIdQueryHandler _sut;

    public GetEvaluatedParametersByPatientIdQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<LabParameterEvaluatedLabParameterMappingProfile>();
        });

        mapperConfig.AssertConfigurationIsValid();

        _mapper = mapperConfig.CreateMapper();

        _mockLabParametersService = new();

        _sut = new GetEvaluatedParametersByPatientIdQueryHandler(
            _mapper,
            _mockLabParametersService.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectly()
    {
        // Arrange
        int patientId = 123;
        var query = new GetEvaluatedParametersByPatientIdQuery(patientId);

        var sourceData = new List<LabParameter>
        {
            new() { Id = 1, Name = "Hemoglobin" },
            new() { Id = 2, Name = "Cholesterol" }
        };

        _mockLabParametersService
            .Setup(x => x.GetAllByPatientIdAsync(patientId))
            .ReturnsAsync(sourceData);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.EvaluatedLabParameters);
        Assert.Equal(2, result.EvaluatedLabParameters.Count);

        Assert.Equal(1, result.EvaluatedLabParameters[0].Id);
        Assert.Equal("Hemoglobin", result.EvaluatedLabParameters[0].Name);

        Assert.Equal(2, result.EvaluatedLabParameters[1].Id);
        Assert.Equal("Cholesterol", result.EvaluatedLabParameters[1].Name);
    }
}