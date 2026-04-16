namespace Contracts;

public class InventoryReservedEvent
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
}