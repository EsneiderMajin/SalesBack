using System.Collections.Generic;
using SalesBack.Models;

namespace SalesBack.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetActiveProducts();
    }
}
