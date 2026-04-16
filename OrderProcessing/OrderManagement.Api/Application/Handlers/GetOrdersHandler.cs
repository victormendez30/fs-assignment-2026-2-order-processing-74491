using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Application.DTOs;
using OrderManagement.Api.Application.Queries;
using OrderManagement.Api.Persistence;

namespace OrderManagement.Api.Application.Handlers;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly OrderDbContext _context;
    private readonly IMapper _mapper;

    public GetOrdersHandler(OrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<OrderDto>>(orders);
    }
}