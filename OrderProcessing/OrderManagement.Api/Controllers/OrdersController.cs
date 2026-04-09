using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Application.Commands;
using OrderManagement.Api.Application.Queries;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var result = await _mediator.Send(new GetOrdersQuery());
        return Ok(result);
    }
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return Ok(orderId);
    }
}
