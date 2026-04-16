namespace Contracts;

public class ShippingCreatedEvent
{
    public Guid OrderId { get; set; }
    public string ShipmentReference { get; set; } = string.Empty;
    public DateTime EstimatedDispatchDate { get; set; }
}