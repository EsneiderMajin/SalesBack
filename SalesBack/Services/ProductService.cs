using System.Collections.Generic;
using System.Linq;
using SalesBack.Data;
using SalesBack.Models;
namespace SalesBack.Services
{
    public class ProductService : IProductService
    {
        public IEnumerable<Product> GetActiveProducts()
        {
            using (var db = new SalesDbContext())
            {
                return db.Products.Where(p => p.IsActive).ToList();
            }
        }
    }

}