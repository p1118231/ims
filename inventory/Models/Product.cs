using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inventory.Models;

public class Product{

    public int Id{get; set;}

    public String? Name{get; set;} 

    public int Quantity {get; set;}

    public String? Category {get; set;}

    public String? ImageUrl {get; set;}

    public String? Description {get; set;}

    [Column(TypeName = "decimal(18, 2)")]

    public decimal Price {get;set;}

    public String? Supplier{get; set;}


}