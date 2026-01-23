using AutoMapper;
using InsightMed.Application.Modules.Patients.Mapping;
using InsightMed.Application.Modules.Patients.Queries;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Application.Options;
using InsightMed.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace InsightMed.UnitTests.Application.QueryHandlers;

public sealed class GetAllPatientsQueryHanlderTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IPatientsService> _mockPatientsService;
    private readonly Mock<IOptions<PagingOptions>> _mockPagingOptions;

    private readonly GetAllPatientsQueryHanlder _sut;

    private const int DefaultPageSize = 10;

    public GetAllPatientsQueryHanlderTests()
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
            .Returns(new PagingOptions { PatientsPageSize = DefaultPageSize });

        _sut = new GetAllPatientsQueryHanlder(
            _mockPatientsService.Object,
            _mapper,
            _mockPagingOptions.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldUsePagingService_WhenNoSearchKey_AndPageNumberProvided()
    {
        // Arrange
        int requestPageNumber = 2;
        var query = new GetAllPatientsQuery(SearchKey: null, PageNumber: requestPageNumber);

        var patientsFromDb = new List<Patient>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe" }
        };

        int totalDbCount = 50;

        _mockPatientsService
            .Setup(x => x.GetAllPagedAsync(requestPageNumber, DefaultPageSize))
            .ReturnsAsync((patientsFromDb, totalDbCount));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        _mockPatientsService.Verify(x => x.GetAllPagedAsync(requestPageNumber, DefaultPageSize), Times.Once);
        _mockPatientsService.Verify(x => x.GetAllAsync(), Times.Never);

        Assert.Equal(requestPageNumber, result.PageNumber);
        Assert.Equal(DefaultPageSize, result.PageSize);
        Assert.Equal(totalDbCount, result.TotalCount);
        Assert.Single(result.Patients);
        Assert.Equal("John", result.Patients[0].FirstName);
    }

    [Fact]
    public async Task Handle_ShouldReturnAll_WhenNoSearchKey_AndPageNumberIsZeroOrNull()
    {
        // Arrange
        var query = new GetAllPatientsQuery(SearchKey: string.Empty, PageNumber: 0);

        var patientsFromDb = new List<Patient>
        {
            new() { Id = 1, FirstName = "Alice" },
            new() { Id = 2, FirstName = "Bob" }
        };

        int totalDbCount = 2;

        _mockPatientsService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync((patientsFromDb, totalDbCount));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        _mockPatientsService.Verify(x => x.GetAllAsync(), Times.Once);
        _mockPatientsService.Verify(x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(totalDbCount, result.PageSize);
        Assert.Equal(totalDbCount, result.TotalCount);
        Assert.Equal(2, result.Patients.Count);
    }

    [Fact]
    public async Task Handle_ShouldSearchByTokens_WhenSearchKeyIsProvided()
    {
        // Arrange
        string searchKey = " John Smith ";
        int requestPageNumber = 1;
        var query = new GetAllPatientsQuery(SearchKey: searchKey, PageNumber: requestPageNumber);

        var patientsFromDb = new List<Patient>
        {
            new() { Id = 5, FirstName = "John", LastName = "Smith" }
        };

        int totalDbCount = 1;

        _mockPatientsService
            .Setup(x => x.SearchByTokensPagedAsync(
                It.Is<string[]>(tokens => tokens.Length == 2 && tokens[0].Equals("John") && tokens[1].Equals("Smith")),
                requestPageNumber,
                DefaultPageSize))
            .ReturnsAsync((patientsFromDb, totalDbCount));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        _mockPatientsService.Verify(x => x.SearchByTokensPagedAsync(It.IsAny<string[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        Assert.Single(result.Patients);
        Assert.Equal("John", result.Patients[0].FirstName);
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultPageOne_WhenSearchKeyProvided_ButPageNumberIsNull()
    {
        // Arrange
        string searchKey = "Test";
        var query = new GetAllPatientsQuery(SearchKey: searchKey, PageNumber: null);

        _mockPatientsService
            .Setup(x => x.SearchByTokensPagedAsync(It.IsAny<string[]>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(([], 0));

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert
        _mockPatientsService.Verify(x => x.SearchByTokensPagedAsync(
            It.IsAny<string[]>(),
            1,
            DefaultPageSize), Times.Once);
    }
}