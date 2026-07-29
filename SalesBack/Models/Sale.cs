using System;
using System.Collections.Generic;

namespace SalesBack.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; }
    }
}
