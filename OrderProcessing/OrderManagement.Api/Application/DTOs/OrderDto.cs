namespace OrderManagement.Api.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public string InventoryStatus { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public string ShippingStatus { get; set; } = string.Empty;

    public string ShipmentReference { get; set; } = string.Empty;

    public DateTime? EstimatedDispatchDate { get; set; }

    public string FailureReason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}