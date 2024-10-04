using LhDev.DemoOne.Models;
using Microsoft.EntityFrameworkCore;

namespace LhDev.DemoOne.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().ToTable("Customer");
    }
}