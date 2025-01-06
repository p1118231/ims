using System;
using System.Collections.Generic;
using inventory.Models.Orders;

namespace inventory.Models.Orders
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemViewModel>? Items { get; set; }
    }
}
