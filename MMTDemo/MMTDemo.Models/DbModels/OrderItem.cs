using System;
using System.Collections.Generic;
using System.Text;

namespace MMTDemo.Models.DbModels
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Decimal Price { get; set; }
        public bool Returnable { get; set; }
    }
}
