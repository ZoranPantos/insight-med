using AutoMapper;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.Patients.Commands;
using InsightMed.Application.Modules.Patients.Mapping;
using InsightMed.Application.Modules.Patients.Models;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using Moq;

namespace InsightMed.UnitTests.Application.CommandHandlers;

public sealed class UpdatePatientCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IPatientsService> _mockPatientsService;

    private readonly UpdatePatientCommandHandler _sut;

    public UpdatePatientCommandHandlerTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdatePatientCommandInputUpdatePatientDtoProfile>();
        });

        _mapper = mapperConfig.CreateMapper();
        _mockPatientsService = new Mock<IPatientsService>();

        _sut = new UpdatePatientCommandHandler(_mapper, _mockPatientsService.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientDoesNotExist()
    {
        // Arrange
        var input = new UpdatePatientCommandInput { Id = 99 };
        var command = new UpdatePatientCommand(input);

        _mockPatientsService
            .Setup(x => x.UpdateAsync(input.Id, It.IsAny<UpdatePatientDto>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            _sut.Handle(command, CancellationToken.None));

        _mockPatientsService.Verify(
            x => x.UpdateAsync(input.Id, It.IsAny<UpdatePatientDto>()),
            Times.Once);
    }
}