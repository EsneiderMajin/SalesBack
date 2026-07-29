using System;
using System.Collections.Generic;
using SalesBack.Data;
using SalesBack.Models;
using System.Linq;


namespace SalesBack.Services
{
    public class CustomerService : ICustomerService
    {
        public IEnumerable<Customer> GetActiveCustomers()
        {
            using (var db = new SalesDbContext())
            {
                return db.Customers.Where(c => c.IsActive).ToList();
            }
        }
    }

}
