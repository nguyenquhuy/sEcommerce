using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the DbContext so the Application layer can query/persist
/// without depending on Infrastructure. Implemented by AppDbContext.
/// </summary>
public interface IAppDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<Inventory> Inventories { get; }

    DbSet<User> Users { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Coupon> Coupons { get; }

    DbSet<Cart> Carts { get; }
    DbSet<CartItem> CartItems { get; }

    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Shipment> Shipments { get; }
    DbSet<OrderAuditLog> OrderAuditLogs { get; }

    DbSet<Review> Reviews { get; }

    DbSet<ReturnRequest> ReturnRequests { get; }
    DbSet<ReturnItem> ReturnItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
