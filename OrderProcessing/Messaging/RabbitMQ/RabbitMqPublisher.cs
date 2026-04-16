using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Messaging.RabbitMQ;

public class RabbitMqPublisher
{
    public void Publish<T>(string queueName, T message)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[RabbitMQ] Published to queue '{queueName}': {json}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RabbitMQ ERROR] Failed to publish to '{queueName}': {ex.Message}");
            throw;
        }
    }
}