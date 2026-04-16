using Inventory.Service.RabbitMQ;

var consumer = new OrderCreatedConsumer();
consumer.Start();

Console.ReadLine();