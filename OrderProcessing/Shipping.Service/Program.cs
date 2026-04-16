using Shipping.Service.RabbitMQ;

var consumer = new PaymentProcessedConsumer();
consumer.Start();

Console.ReadLine();