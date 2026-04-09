using MediatR;
using OrderManagement.Api.Application.Queries;
using OrderManagement.Api.Domain;
using static OrderManagement.Api.Application.Handlers.CreateOrderHandler;

namespace OrderManagement.Api.Application.Handlers;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<Order>>
{
   
     public Task<List<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(OrderStore.Orders);
    }
}