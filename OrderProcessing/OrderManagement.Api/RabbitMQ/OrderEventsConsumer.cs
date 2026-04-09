using Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrderManagement.Api.Application.Handlers;

namespace OrderManagement.Api.RabbitMQ;

public class OrderEventsConsumer
{
    public void Start()
    {
        try
        {
            Console.WriteLine("Starting OrderEventsConsumer...");

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            // =============================
            // INVENTORY EVENT
            // =============================
            channel.QueueDeclare(
                queue: "inventory-reserved",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var inventoryConsumer = new EventingBasicConsumer(channel);

            inventoryConsumer.Received += (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                var message = JsonSerializer.Deserialize<InventoryReservedEvent>(json);

                var order = OrderStore.Orders.FirstOrDefault(o => o.Id == message!.OrderId);

                if (order != null)
                {
                    order.Status = "InventoryReserved";
                    Console.WriteLine($"[OrderAPI] Order {order.Id} → InventoryReserved");
                }
            };

            channel.BasicConsume(
                queue: "inventory-reserved",
                autoAck: true,
                consumer: inventoryConsumer
            );

            // =============================
            // PAYMENT EVENT
            // =============================
            channel.QueueDeclare(
                queue: "payment-processed",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var paymentConsumer = new EventingBasicConsumer(channel);

            paymentConsumer.Received += (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                var message = JsonSerializer.Deserialize<PaymentProcessedEvent>(json);

                var order = OrderStore.Orders.FirstOrDefault(o => o.Id == message!.OrderId);

                if (order != null)
                {
                    order.Status = "Shipped";
                    Console.WriteLine($"[OrderAPI] Order {order.Id} → Shipped");
                }
            };

            channel.BasicConsume(
                queue: "payment-processed",
                autoAck: true,
                consumer: paymentConsumer
            );

            Console.WriteLine("Order API listening to events...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }
}