using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cotrust.Models;

public partial class CotrustDbContext : DbContext
{
    public CotrustDbContext() { }

    public CotrustDbContext(DbContextOptions<CotrustDbContext> options) : base(options) { }

    public DbSet<User> User { get; set; }
    public DbSet<Direction> Directions { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }
    public DbSet<Buys> Buys { get; set; }
    public DbSet<BuysProduct> BuysProducts { get; set; }
    public DbSet<Package> Package { get; set; }
    public DbSet<PackageProduct> PackageProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
