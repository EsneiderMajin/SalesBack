using System;
using System.Collections.Generic;

namespace SalesBack.DTOs
{
    public class SaleDetailResponse
    {
        public int SaleId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SaleDetailItemResponse> Items { get; set; }
    }

    public class SaleDetailItemResponse
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
