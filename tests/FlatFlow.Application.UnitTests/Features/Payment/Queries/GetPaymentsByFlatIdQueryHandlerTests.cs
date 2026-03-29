using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Payment.Queries.GetPaymentsByFlatId;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Payment.Queries;

public class GetPaymentsByFlatIdQueryHandlerTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly GetPaymentsByFlatIdQueryHandler _handler;

    public GetPaymentsByFlatIdQueryHandlerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PaymentMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetPaymentsByFlatIdQueryHandler(_paymentRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_WithPayments_ShouldReturnPaginatedResult()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment1 = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());
        var payment2 = flat.AddPayment("Prąd", 300m, DateTime.UtcNow, Guid.NewGuid());
        var paginatedResult = new PaginatedResult<Domain.Entities.Payment>(
            [payment1, payment2], TotalCount: 5, Page: 1, PageSize: 2);

        _paymentRepositoryMock
            .Setup(r => r.GetByFlatIdPaginatedAsync(flat.Id, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var query = new GetPaymentsByFlatIdQuery(flat.Id, Page: 1, PageSize: 2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        var paginatedResult = new PaginatedResult<Domain.Entities.Payment>(
            [], TotalCount: 0, Page: 1, PageSize: 10);

        _paymentRepositoryMock
            .Setup(r => r.GetByFlatIdPaginatedAsync(flatId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var query = new GetPaymentsByFlatIdQuery(flatId, Page: 1, PageSize: 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
    }
}
