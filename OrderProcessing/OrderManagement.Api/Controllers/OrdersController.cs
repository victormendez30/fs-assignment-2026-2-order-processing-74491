using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Application.Commands;
using OrderManagement.Api.Application.Queries;
using OrderManagement.Api.Persistence;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly OrderDbContext _context;

    public OrdersController(IMediator mediator, OrderDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var result = await _mediator.Send(new GetOrdersQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetOrderStatus(Guid id)
    {
        var order = await _context.Orders
            .Where(o => o.Id == id)
            .Select(o => new
            {
                o.Id,
                o.Status,
                o.InventoryStatus,
                o.PaymentStatus,
                o.ShippingStatus,
                o.ShipmentReference,
                o.EstimatedDispatchDate,
                o.FailureReason,
                o.CompletedAt
            })
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet("/api/customers/{id}/orders")]
    public async Task<IActionResult> GetCustomerOrders(string id)
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerEmail == id || o.CustomerName == id)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return Ok(orderId);
    }
}