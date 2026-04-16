using Contracts;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrderManagement.Api.Persistence;

namespace OrderManagement.Api.RabbitMQ;

public class OrderEventsConsumer
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderEventsConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void Start()
    {
        Console.WriteLine("Starting OrderEventsConsumer...");

        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "inventory-reserved-orderapi",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var inventoryConsumer = new EventingBasicConsumer(channel);

        inventoryConsumer.Received += async (model, ea) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var message = JsonSerializer.Deserialize<InventoryReservedEvent>(
                Encoding.UTF8.GetString(ea.Body.ToArray())
            );

            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == message!.OrderId);

            if (order != null)
            {
                if (message.Success)
                {
                    order.InventoryStatus = "Confirmed";
                    order.Status = "InventoryConfirmed";
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[OrderAPI] {order.Id} → InventoryConfirmed");

                    order.PaymentStatus = "Pending";
                    order.Status = "PaymentPending";
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[OrderAPI] {order.Id} → PaymentPending");
                }
                else
                {
                    order.InventoryStatus = "Failed";
                    order.Status = "Failed";
                    order.FailureReason = "Inventory validation failed";
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[OrderAPI] {order.Id} → Failed (Inventory)");
                }
            }
        };

        channel.BasicConsume(
            queue: "inventory-reserved-orderapi",
            autoAck: true,
            consumer: inventoryConsumer
        );

        channel.QueueDeclare(
            queue: "payment-processed-orderapi",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var paymentConsumer = new EventingBasicConsumer(channel);

        paymentConsumer.Received += async (model, ea) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var message = JsonSerializer.Deserialize<PaymentProcessedEvent>(
                Encoding.UTF8.GetString(ea.Body.ToArray())
            );

            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == message!.OrderId);

            if (order != null)
            {
                if (message.Success)
                {
                    order.PaymentStatus = "Approved";
                    order.Status = "PaymentApproved";
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[OrderAPI] {order.Id} → PaymentApproved");

                    order.ShippingStatus = "Pending";
                    order.Status = "ShippingPending";
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[OrderAPI] {order.Id} → ShippingPending");
                }
                else
                {
                    order.PaymentStatus = "Rejected";
                    order.Status = "Failed";
                    order.FailureReason = "Payment processing failed";
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[OrderAPI] {order.Id} → Failed (Payment)");
                }
            }
        };

        channel.BasicConsume(
            queue: "payment-processed-orderapi",
            autoAck: true,
            consumer: paymentConsumer
        );

        channel.QueueDeclare(
            queue: "shipping-created-orderapi",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var shippingConsumer = new EventingBasicConsumer(channel);

        shippingConsumer.Received += async (model, ea) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var message = JsonSerializer.Deserialize<ShippingCreatedEvent>(
                Encoding.UTF8.GetString(ea.Body.ToArray())
            );

            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == message!.OrderId);

            if (order != null)
            {
                order.ShippingStatus = "Created";
                order.ShipmentReference = message.ShipmentReference;
                order.EstimatedDispatchDate = message.EstimatedDispatchDate;
                order.Status = "ShippingCreated";
                await context.SaveChangesAsync();

                Console.WriteLine($"[OrderAPI] {order.Id} → ShippingCreated");

                order.Status = "Completed";
                order.CompletedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                Console.WriteLine($"[OrderAPI] {order.Id} → Completed");
            }
        };

        channel.BasicConsume(
            queue: "shipping-created-orderapi",
            autoAck: true,
            consumer: shippingConsumer
        );

        Console.WriteLine("Order API listening to events...");
    }
}