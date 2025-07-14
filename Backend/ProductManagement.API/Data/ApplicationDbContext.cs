using Microsoft.EntityFrameworkCore;
using ProductManagement.API.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProductPrice>()
            .HasOne(pp => pp.Product)
            .WithMany(p => p.ProductPrices)
            .HasForeignKey(pp => pp.ProductId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Customer)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.CustomerId);

        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(d => d.Invoice)
            .WithMany(i => i.InvoiceDetails)
            .HasForeignKey(d => d.InvoiceId);

        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(d => d.Product)
            .WithMany()
            .HasForeignKey(d => d.ProductId);
    }
}
