using AutoMapper;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Mapping;
using InsightMed.Application.Modules.Patients.Queries;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Application.Options;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using Microsoft.Extensions.Options;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetPatientByIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IPatientsService> _mockPatientsService;
    private readonly Mock<IOptions<PagingOptions>> _mockPagingOptions;

    private readonly GetPatientByIdQueryHandler _handler;

    public GetPatientByIdQueryHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PatientMappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _mockPatientsService = new();
        _mockPagingOptions = new();

        _mockPagingOptions
            .Setup(x => x.Value)
            .Returns(new PagingOptions { PatientDetailsLabRequestsPageSize = 5 });

        _handler = new GetPatientByIdQueryHandler(
            _mapper,
            _mockPatientsService.Object,
            _mockPagingOptions.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowResourceNotFoundException_WhenPatientIsNull()
    {
        // Arrange
        int patientId = 999;
        var query = new GetPatientByIdQuery(patientId, 1);

        _mockPatientsService
            .Setup(x => x.GetByIdWithLabRequestsPagedAsync(patientId, 1, 5))
            .ReturnsAsync((null, [], 0));

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectlyMappedResponse_WhenPatientExists()
    {
        // Arrange
        int patientId = 10;
        int pageNumber = 1;
        int pageSize = 5;
        var query = new GetPatientByIdQuery(patientId, pageNumber);

        var patientFromDb = new Patient
        {
            Id = patientId,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Gender = Gender.Female
        };


        var reqCompleted = new LabRequest
        {
            Id = 1,
            LabRequestState = LabRequestState.Completed,
            LabReport = new LabReport { Id = 100 }
        };

        var reqInProgress = new LabRequest
        {
            Id = 2,
            LabRequestState = LabRequestState.Pending,
            LabReport = new LabReport { Id = 200 }
        };

        var reqNoReport = new LabRequest
        {
            Id = 3,
            LabRequestState = LabRequestState.Completed,
            LabReport = null
        };

        var pagedRequests = new List<LabRequest> { reqCompleted, reqInProgress, reqNoReport };
        int totalCount = 50;

        _mockPatientsService
            .Setup(x => x.GetByIdWithLabRequestsPagedAsync(patientId, pageNumber, pageSize))
            .ReturnsAsync((patientFromDb, pagedRequests, totalCount));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(patientId, result.Id);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("jane@example.com", result.Email);

        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(totalCount, result.TotalCount);

        Assert.Equal(3, result.LabRequests.Count);

        var resCompleted = result.LabRequests.Find(r => r.Id == 1);
        Assert.Equal(100, resCompleted?.LabReportId);

        var resInProgress = result.LabRequests.Find(r => r.Id == 2);
        Assert.Null(resInProgress?.LabReportId);

        var resNoReport = result.LabRequests.Find(r => r.Id == 3);
        Assert.Null(resNoReport?.LabReportId);
    }
}