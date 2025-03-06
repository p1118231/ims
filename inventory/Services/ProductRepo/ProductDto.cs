using System;
using Microsoft.VisualBasic;

namespace inventory.Services.ProductRepo;

public class ProductDto{
    public int Id{get; set;}

    public String? Name{get; set;} 

    public int Quantity {get; set;}

    public String? Category {get; set;}

    public String? ImageUrl {get; set;}

    public String? Description {get; set;}

    public decimal Price {get;set;}

    public String? Supplier{get; set;}

   
}