using Contracts;
using Messaging.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Inventory.Service.RabbitMQ;

public class OrderCreatedConsumer
{
    private readonly RabbitMqPublisher _publisher = new();

    public void Start()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "order-created",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<OrderCreatedEvent>(json);

            Console.WriteLine($"[Inventory] Order received: {message?.OrderId}");

            var success = true;

            //  publish to Order API
            _publisher.Publish("inventory-reserved-orderapi", new InventoryReservedEvent
            {
                OrderId = message!.OrderId,
                Success = success
            });

            //  publish to Payment Service
            _publisher.Publish("inventory-reserved-payment", new InventoryReservedEvent
            {
                OrderId = message!.OrderId,
                Success = success
            });
        };

        channel.BasicConsume(
            queue: "order-created",
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine("Inventory Service is listening...");
    }
}