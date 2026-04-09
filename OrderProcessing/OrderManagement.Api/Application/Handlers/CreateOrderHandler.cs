using MediatR;
using OrderManagement.Api.Application.Commands;
using OrderManagement.Api.Domain;
using Messaging.RabbitMQ;
using Contracts;

namespace OrderManagement.Api.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly RabbitMqPublisher _publisher = new();
    public static class CreateOrderHandlerAccessor
    {
        public static List<Order> Orders { get; } = new();
    }

    public Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            TotalAmount = request.TotalAmount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        OrderStore.Orders.Add(order);
        _publisher.Publish("order-created", new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            TotalAmount = order.TotalAmount
        });

        return Task.FromResult(order.Id);
    }
}