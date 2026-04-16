using Contracts;
using Messaging.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Shipping.Service.RabbitMQ;

public class PaymentProcessedConsumer
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
            queue: "payment-processed-shipping",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<PaymentProcessedEvent>(json);

            await Task.Delay(2000);

            Console.WriteLine($"[Shipping] Shipping created: {message?.OrderId}");

            _publisher.Publish("shipping-created-orderapi", new ShippingCreatedEvent
            {
                OrderId = message!.OrderId,
                ShipmentReference = $"SHIP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                EstimatedDispatchDate = DateTime.UtcNow.AddDays(2)
            });
        };

        channel.BasicConsume(
            queue: "payment-processed-shipping",
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine("Shipping Service is listening...");
    }
}