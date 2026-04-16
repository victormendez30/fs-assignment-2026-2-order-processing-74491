using MediatR;

namespace OrderManagement.Api.Application.Commands;

public class CreateOrderCommand : IRequest<Guid>
{
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}