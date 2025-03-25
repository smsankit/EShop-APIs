using Microsoft.EntityFrameworkCore;
using Products.Models;
using System.Collections.Generic;

namespace Products.Data
{
    public class ProductsDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductsDbContext(DbContextOptions options) :base(options)
        {
            Products = Set<Product>();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToContainer("Products").HasPartitionKey(k => k.Id);
            modelBuilder.Entity<Rating>().ToContainer("Ratings").HasPartitionKey(k => k.Id);
        }
    }
}