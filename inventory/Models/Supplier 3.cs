namespace inventory.Models{
    public class Supplier{
        public int SupplierId{get; set;}
        public string? Name{get; set;}
        public string? Address{get; set;}
        public string? Email{get; set;}
        public string? Phone{get; set;}

        public required ICollection<Product> Products { get; set; }
    }
}