using System.Collections.Generic;

namespace SalesBack.DTOs
{
    public class CreateSaleRequest
    {
        public int CustomerId { get; set; }
        public List<SaleItemRequest> Items { get; set; }
    }
}
