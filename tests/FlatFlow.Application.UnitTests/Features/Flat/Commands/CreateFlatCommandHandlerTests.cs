using FlatFlow.Application.Common.Models.Identity;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Commands.CreateFlat;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Commands;

public class CreateFlatCommandHandlerTests
{
    private const string TestUserId = "test-user-id";
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly CreateFlatCommandHandler _handler;
    private Domain.Entities.Flat? _capturedFlat;

    public CreateFlatCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _authServiceMock = new Mock<IAuthService>();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(TestUserId);
        _authServiceMock.Setup(s => s.GetUserAsync(TestUserId))
            .ReturnsAsync(new UserProfile { UserId = TestUserId, FirstName = "Jan", LastName = "Kowalski", Email = "jan@test.com" });
        _flatRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Flat>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat f, CancellationToken _) =>
            {
                _capturedFlat = f;
                return f;
            });

        _handler = new CreateFlatCommandHandler(
            _flatRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _authServiceMock.Object,
            Mock.Of<ILogger<CreateFlatCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnCreatedFlatId()
    {
        // Arrange
        var command = new CreateFlatCommand("Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _capturedFlat.Should().NotBeNull();
        result.Should().Be(_capturedFlat!.Id);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateFlatWithCorrectProperties()
    {
        // Arrange
        var command = new CreateFlatCommand("Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _flatRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Domain.Entities.Flat>(f =>
                f.Name == "Mieszkanie" &&
                f.Address.Street == "Długa 5" &&
                f.Address.City == "Kraków" &&
                f.Address.ZipCode == "30-001" &&
                f.Address.Country == "Poland" &&
                f.AccessCode != string.Empty),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldAutoCreateOwnerTenant()
    {
        // Arrange
        var command = new CreateFlatCommand("Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _capturedFlat.Should().NotBeNull();
        var tenant = _capturedFlat!.Tenants.Should().ContainSingle().Subject;
        tenant.FirstName.Should().Be("Jan");
        tenant.LastName.Should().Be("Kowalski");
        tenant.Email.Should().Be("jan@test.com");
        tenant.UserId.Should().Be(TestUserId);
        tenant.IsOwner.Should().BeTrue();
    }
}
