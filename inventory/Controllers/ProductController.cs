using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inventory.Models;
using Microsoft.AspNetCore.Authorization;
using inventory.Services.ProductRepo;
using System.Threading.Tasks;
using inventory.Services.CategoryRepo;
using inventory.Services.SupplierRepo;


namespace inventory.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;

    private readonly IProductService _productService;

    private readonly ICategoryService _categoryService;

    private readonly ISupplierService _supplierService;

    public ProductController(ILogger<ProductController> logger, IProductService productService, ICategoryService categoryService, ISupplierService supplierService)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
        _supplierService = supplierService;
    }
    
    
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
        public async Task<IActionResult> Create()
        {
            try{
            ViewBag.Categories = await _categoryService.GetCategories();
            ViewBag.Suppliers = await _supplierService.GetSuppliers();
            // Return the Create view with an empty product model
            return View();
            }
            catch{
                _logger.LogWarning("failure to create product");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {

            try{
            if (ModelState.IsValid)
            {
                // Call your service or database layer to add the product
                await _productService.AddProduct(product);
                await _productService.SaveChangesAsync();

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }
            // Repopulate dropdowns if form submission fails
            ViewBag.Suppliers = await _supplierService.GetSuppliers();
            ViewBag.Categories = await _categoryService.GetCategories();
            // If validation fails, return the form with the current model data
            return View(product);
            }
            catch{
                _logger.LogWarning("failure to create product");
                return StatusCode(500, "Internal server error");
            }
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

         //add edit field for the product entities
            public async Task<IActionResult> EditField(int id, string field)
            {

                try{
                    var product = await _productService.GetProductByIdAsync(id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    // Fetch dropdown data for suppliers and categories
                    if (field.ToLower() == "supplier" || field.ToLower() == "category")
                    {
                        ViewBag.Categories =await _categoryService.GetCategories();
                        ViewBag.Suppliers = await _supplierService.GetSuppliers();
                    }

                    ViewBag.FieldToEdit = field;

                    ViewBag.FieldValue = field.ToLower() switch
                    {
                        "description" => (object)(product.Description ?? ""),
                        "supplier" => (object)(product.Supplier?.Name ?? ""),
                        "category" => (object)(product.Category?.Name ?? ""),
                        "quantity" => (object)product.Quantity,
                        "price" => (object)product.Price,
                        "image" => (object)(product.ImageUrl ?? ""),
                        _ => null
                    };

                    return View(product);
                }
                catch{
                    _logger.LogWarning("failure to get product details");
                    return StatusCode(500, "Internal server error");
                }
               

               
            }

            [HttpPost]
            public async Task<IActionResult> EditField(int id, string field, string newValue, int? restockAmount)
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound();
                }


                if (string.IsNullOrEmpty(newValue))
                {
                    ModelState.AddModelError("", "The new value cannot be empty.");
                    ViewBag.FieldToEdit = field; // Ensure FieldToEdit is set
                    ViewBag.Categories =await _categoryService.GetCategories();
                    ViewBag.Suppliers = await _supplierService.GetSuppliers();
                    return View(product);
                }

                switch (field.ToLower())
                {
                    case "description":
                        product.Description = newValue;
                        break;
                    case "supplier":
                        product.SupplierId = int.Parse(newValue); 
                        break;
                    case "category":
                        product.CategoryId = int.Parse(newValue); 
                        break;
                    case "price":
                        product.Price = decimal.Parse(newValue);
                        break;
                    case "quantity":
                        product.Quantity = int.Parse(newValue);
                        break;
                    case "restock":
                        if (restockAmount.HasValue && restockAmount.Value > 0)
                        {
                            product.Quantity += restockAmount.Value; // Add restock amount to existing quantity
                        }
                        else
                        {
                            ModelState.AddModelError("", "Restock amount must be greater than zero.");
                            return View(product);
                        }
                        break;
                    case "image":
                        product.ImageUrl = newValue;
                        break;
                    default:
                        return BadRequest("Invalid field");
                }

                await _productService.UpdateProduct(product);
                await _productService.SaveChangesAsync();

                TempData["SuccessMessage"] = "Changes have been saved successfully!";

                return RedirectToAction("Details", new { id });
            }

    
        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product =  await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                await _productService.RemoveProduct(product);
            }

            await _productService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


}
        