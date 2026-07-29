using System.Data.Entity;
using SalesBack.Models;

namespace SalesBack.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext() : base("name=SalesDbContext")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
    }
}
