namespace Contracts;

public class PaymentProcessedEvent
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
}