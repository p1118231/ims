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
    public async Task<IActionResult> Index(string? query)
    {
        IEnumerable<Product> products = null!;

            try{

            if (!string.IsNullOrEmpty(query))
            {
                // Fetch products that match the search query
                var allProducts = await _productService.GetProducts();
                products = allProducts?.Where(p => (p.Name??"").Contains(query, StringComparison.OrdinalIgnoreCase)) ?? Enumerable.Empty<Product>();
            }
            else
            {
                // Fetch all products
                products = await _productService.GetProducts();
            }

            // Pass the query back to the view for display in the search box
            ViewData["SearchQuery"] = query;

            }
            catch (Exception ex){

                _logger.LogWarning($"failure to access product service : {ex.Message}");
                products= Array.Empty<Product>();

            }

            return View(products);
    }

            [HttpGet]
        public IActionResult Create()
        {
            // Return the Create view with an empty product model
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Call your service or database layer to add the product
                await _productService.AddProduct(product);

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, return the form with the current model data
            return View(product);
        }

         public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product? product = null;

            try{

             product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            } 
            catch{
                _logger.LogWarning("failure to get product details");
                return StatusCode(500, "Internal server error");
            }

            return View(product);

            
        }
}
