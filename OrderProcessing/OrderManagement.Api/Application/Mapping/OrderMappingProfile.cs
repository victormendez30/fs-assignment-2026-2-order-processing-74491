using AutoMapper;
using OrderManagement.Api.Application.DTOs;
using OrderManagement.Api.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderManagement.Api.Application.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}