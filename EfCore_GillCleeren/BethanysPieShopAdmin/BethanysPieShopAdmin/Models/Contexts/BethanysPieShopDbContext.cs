using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models.Contexts;

public class BethanysPieShopDbContext: DbContext
{
    public BethanysPieShopDbContext(DbContextOptions<BethanysPieShopDbContext> opt) : base(opt)
    {
        
    }
    
    public DbSet<Pie> Pies { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies configuration for all IEntityTypeConfiguration<T> instances that are defined in provided assembly 
        // AKA --> class CategoryEntityTypeConfiguration: IEntityTypeConfiguration<Category>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BethanysPieShopDbContext).Assembly);
        
        modelBuilder.Entity<Pie>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Pies)
            .HasForeignKey(p => p.CategoryId);
        
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);
    }
}