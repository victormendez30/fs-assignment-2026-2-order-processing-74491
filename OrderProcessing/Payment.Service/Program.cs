using Payment.Service.RabbitMQ;

var consumer = new InventoryReservedConsumer();
consumer.Start();

Console.ReadLine();