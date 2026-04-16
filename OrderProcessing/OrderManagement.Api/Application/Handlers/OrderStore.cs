using OrderManagement.Api.Domain;

namespace OrderManagement.Api.Application.Handlers;

public static class OrderStore
{
    public static List<Order> Orders { get; } = new();
}