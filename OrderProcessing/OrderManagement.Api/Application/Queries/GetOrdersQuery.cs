using MediatR;
using OrderManagement.Api.Application.DTOs;

namespace OrderManagement.Api.Application.Queries;

public class GetOrdersQuery : IRequest<List<OrderDto>>
{
}