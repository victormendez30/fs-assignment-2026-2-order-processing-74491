using Contracts;
using Messaging.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Payment.Service.RabbitMQ;

public class InventoryReservedConsumer
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
            queue: "inventory-reserved",
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

            var message = JsonSerializer.Deserialize<InventoryReservedEvent>(json);

            Console.WriteLine($"[Payment] Inventory result for order: {message?.OrderId}");

            var success = true;

            _publisher.Publish("payment-processed", new PaymentProcessedEvent
            {
                OrderId = message!.OrderId,
                Success = success
            });
        };

        channel.BasicConsume(
            queue: "inventory-reserved",
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine("Payment Service is listening...");
    }
}