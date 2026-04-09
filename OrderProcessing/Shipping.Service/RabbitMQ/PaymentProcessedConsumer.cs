using Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Shipping.Service.RabbitMQ;

public class PaymentProcessedConsumer
{
    public void Start()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "payment-processed",
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

            var message = JsonSerializer.Deserialize<PaymentProcessedEvent>(json);

            Console.WriteLine($"[Shipping] Order shipped: {message?.OrderId}");
        };

        channel.BasicConsume(
            queue: "payment-processed",
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine("Shipping Service is listening...");
    }
}