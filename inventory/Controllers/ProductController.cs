using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inventory.Models;
using Microsoft.AspNetCore.Authorization;
using inventory.Services.ProductRepo;


namespace inventory.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;

    private readonly IProductService _productService;

    public ProductController(ILogger<ProductController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
        
    }
 
    //get the products from the website 
    public async Task<IActionResult> Index()
    {
        IEnumerable<Product> products = null!;

            try{

                products = await  _productService.GetProducts();

            }
            catch (Exception ex){

                _logger.LogWarning($"failure to access product service : {ex.Message}");
                products= Array.Empty<Product>();

            }

            return View(products);
    }

    //[Authorize]
    
}
