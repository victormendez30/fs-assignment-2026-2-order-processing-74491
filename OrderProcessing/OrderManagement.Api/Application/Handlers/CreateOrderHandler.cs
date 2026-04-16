using Contracts;
using MediatR;
using Messaging.RabbitMQ;
using OrderManagement.Api.Application.Commands;
using OrderManagement.Api.Domain;
using OrderManagement.Api.Persistence;

namespace OrderManagement.Api.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly RabbitMqPublisher _publisher = new();
    private readonly OrderDbContext _context;

    public CreateOrderHandler(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            TotalAmount = request.TotalAmount,
            Status = "Submitted",
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        order.Status = "InventoryPending";
        await _context.SaveChangesAsync(cancellationToken);

        _publisher.Publish("order-created", new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            TotalAmount = order.TotalAmount
        });

        return order.Id;
    }
}