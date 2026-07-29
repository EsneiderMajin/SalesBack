using System;

namespace SalesBack.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
