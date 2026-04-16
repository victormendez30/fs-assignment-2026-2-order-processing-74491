using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Domain;

namespace OrderManagement.Api.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
}