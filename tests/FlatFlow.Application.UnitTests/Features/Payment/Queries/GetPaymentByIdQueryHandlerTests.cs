using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Payment.Queries.GetPaymentById;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Payment.Queries;

public class GetPaymentByIdQueryHandlerTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly GetPaymentByIdQueryHandler _handler;

    public GetPaymentByIdQueryHandlerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PaymentMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetPaymentByIdQueryHandler(_paymentRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingPayment_ShouldReturnPaymentDetailDto()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var createdById = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(30);
        var payment = flat.AddPayment("Czynsz", 1500m, dueDate, createdById);
        var tenantId = Guid.NewGuid();
        payment.AddShare(tenantId, 500m);

        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var query = new GetPaymentByIdQuery(payment.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(payment.Id);
        result.Title.Should().Be("Czynsz");
        result.Amount.Should().Be(1500m);
        result.DueDate.Should().Be(dueDate);
        result.CreatedById.Should().Be(createdById);
        result.Shares.Should().ContainSingle();
        result.Shares[0].TenantId.Should().Be(tenantId);
        result.Shares[0].ShareAmount.Should().Be(500m);
        result.Shares[0].Status.Should().Be(PaymentShareStatus.New);
    }

    [Fact]
    public async Task Handle_NonExistingPayment_ShouldThrowNotFoundException()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Payment?)null);

        var query = new GetPaymentByIdQuery(paymentId);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
