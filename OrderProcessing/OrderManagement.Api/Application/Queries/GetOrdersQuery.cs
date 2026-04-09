using MediatR;
using OrderManagement.Api.Domain;

namespace OrderManagement.Api.Application.Queries;

public class GetOrdersQuery : IRequest<List<Order>>
{
}