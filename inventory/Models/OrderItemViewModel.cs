namespace inventory.Models.Orders
{
    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }
        public String? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public String? ProductImage { get; set; } // URL or path to the product image
    }
}
