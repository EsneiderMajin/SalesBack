using System.Collections.Generic;
using SalesBack.Models;

namespace SalesBack.Services
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetActiveCustomers();
    }
}
