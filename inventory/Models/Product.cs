using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inventory.Models;

public class Product{

    public int ProductId{get; set;}

    public String? Name{get; set;} 

    public int Quantity {get; set;}


    public String? ImageUrl {get; set;}

    public String? Description {get; set;}

    [Column(TypeName = "decimal(18, 2)")]

    public decimal Price {get;set;}

    // Foreign Keys
    public int SupplierId { get; set; }
    public int CategoryId { get; set; }

    // Navigation Properties
    public  Supplier? Supplier { get; set; }
    public  Category? Category { get; set; }


}