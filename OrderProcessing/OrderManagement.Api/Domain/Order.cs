namespace OrderManagement.Api.Domain;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Submitted";

    public string CorrelationId { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = "Pending";

    public string InventoryStatus { get; set; } = "Pending";

    public string ShippingStatus { get; set; } = "Pending";

    public string ShipmentReference { get; set; } = string.Empty;

    public DateTime? EstimatedDispatchDate { get; set; }

    public string FailureReason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }
}