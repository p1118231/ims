using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inventory.Models;
using Microsoft.AspNetCore.Authorization;
using inventory.Services.ProductRepo;
using System.Threading.Tasks;
using inventory.Services.CategoryRepo;
using inventory.Services.SupplierRepo;
using inventory.Services.PriceOptimisation;
using inventory.Services.NotificationRepo;
using Microsoft.AspNetCore.Mvc.Rendering;
using inventory.Services.StockOptimisationRepo;

namespace inventory.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private readonly IPricePredictionService _pricePredictionService;
        private readonly INotificationService _notificationService;
        private readonly IStockOptimisationService _stockOptimisationService;

        public ProductController(ILogger<ProductController> logger, IProductService productService, ICategoryService categoryService, ISupplierService supplierService, IPricePredictionService pricePredictionService, INotificationService notificationService, IStockOptimisationService stockOptimisationService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _pricePredictionService = pricePredictionService;
            _notificationService = notificationService;
            _stockOptimisationService = stockOptimisationService;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index(string? query, int? categoryId, int? supplierId)
        {
            IEnumerable<Product> products = null!;
            IEnumerable<Category> categories = null!;
            IEnumerable<Supplier> suppliers = null!;

            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    // Fetch products that match the search query
                    var allProducts = await _productService.GetProducts();
                    categories = await _categoryService.GetCategories();
                    suppliers = await _supplierService.GetSuppliers();
                    products = allProducts?.Where(p => (p.Name ?? "").Contains(query, StringComparison.OrdinalIgnoreCase)) ?? Enumerable.Empty<Product>();
                }
                else if (categoryId.HasValue)
                {
                    // Fetch products that match the selected category
                    var allProducts = await _productService.GetProducts();
                    categories = await _categoryService.GetCategories();
                    suppliers = await _supplierService.GetSuppliers();
                    products = allProducts?.Where(p => p.CategoryId == categoryId) ?? Enumerable.Empty<Product>();
                }
                else if (supplierId.HasValue)
                {
                    // Fetch products that match the selected supplier
                    var allProducts = await _productService.GetProducts();
                    categories = await _categoryService.GetCategories();
                    suppliers = await _supplierService.GetSuppliers();
                    products = allProducts?.Where(p => p.SupplierId == supplierId) ?? Enumerable.Empty<Product>();
                }
                else
                {
                    // Fetch all products
                    products = await _productService.GetProducts();
                    categories = await _categoryService.GetCategories();
                    suppliers = await _supplierService.GetSuppliers();
                }

                // Pass the query back to the view for display in the search box
                ViewData["SearchQuery"] = query;
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
                ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failure to access product service: {ex.Message}");
                products = Array.Empty<Product>();
            }

            return View(products);
        }

        // GET: Product/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Categories = await _categoryService.GetCategories();
                ViewBag.Suppliers = await _supplierService.GetSuppliers();
                // Return the Create view with an empty product model
                return View();
            }
            catch
            {
                _logger.LogWarning("Failure to create product");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
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
            catch
            {
                _logger.LogWarning("Failure to create product");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product? product = null;
            dynamic predictedPrice = 0.0;
            dynamic predictedStock = 0.0;

            try
            {
                product = await _productService.GetProductByIdAsync(id);

                // Get the predicted price for the product
                predictedPrice = await _pricePredictionService.PredictPriceAsync(id ?? 0);
                ViewBag.PredictedPrice = predictedPrice?.predicted_price ?? 0;

                // Get the predicted stock level for the product
                predictedStock = await _stockOptimisationService.PredictStockLevelAsync(id ?? 0);
                ViewBag.PredictedStock = predictedStock?.predicted_stock_level ?? 0;

                if (product == null)
                {
                    return NotFound();
                }
            }
            catch
            {
                _logger.LogWarning("Failure to get product details");
                return StatusCode(500, "Internal server error");
            }

            return View(product);
        }

        // GET: Product/EditField/5?field=quantity
        public async Task<IActionResult> EditField(int id, string field)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Fetch dropdown data for suppliers and categories
                if (field.ToLower() == "supplier" || field.ToLower() == "category")
                {
                    ViewBag.Categories = await _categoryService.GetCategories();
                    ViewBag.Suppliers = await _supplierService.GetSuppliers();
                }

                ViewBag.FieldToEdit = field;
                ViewBag.FieldValue = field.ToLower() switch
                {
                    "description" => product.Description ?? "",
                    "supplier" => product.Supplier?.Name ?? "",
                    "category" => product.Category?.Name ?? "",
                    "quantity" => product.Quantity.ToString(),
                    "price" => product.Price.ToString(),
                    "image" => product.ImageUrl ?? "",
                    "restock" => "", // No initial value for restock
                    _ => null
                };

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failure to get product details for ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: Product/EditField
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditField(int id, string field, string newValue)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(newValue))
            {
                ModelState.AddModelError("", "The new value cannot be empty.");
                ViewBag.FieldToEdit = field;
                ViewBag.FieldValue = newValue; // Retain submitted value on error
                if (field.ToLower() == "supplier" || field.ToLower() == "category")
                {
                    ViewBag.Categories = await _categoryService.GetCategories();
                    ViewBag.Suppliers = await _supplierService.GetSuppliers();
                }
                return View(product);
            }

            try
            {
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
                        int restockValue = int.Parse(newValue);
                        if (restockValue <= 0)
                        {
                            ModelState.AddModelError("", "Restock amount must be greater than zero.");
                            ViewBag.FieldToEdit = field;
                            ViewBag.FieldValue = newValue; // Retain submitted value
                            return View(product);
                        }
                        product.Quantity += restockValue;
                        break;
                    case "image":
                        product.ImageUrl = newValue;
                        break;
                    default:
                        return BadRequest("Invalid field");
                }

                await _productService.UpdateProduct(product);
                await _productService.SaveChangesAsync();
                await _notificationService.CreateNotificationAsync(new Notification
                {
                    Date = DateTime.Now,
                    Message = $"Product {product.Name} has been updated"
                });

                TempData["SuccessMessage"] = "Changes have been saved successfully!";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes for product {Id}", id);
                ModelState.AddModelError("", "An error occurred while saving changes.");
                ViewBag.FieldToEdit = field;
                ViewBag.FieldValue = newValue; // Retain submitted value on error
                if (field.ToLower() == "supplier" || field.ToLower() == "category")
                {
                    ViewBag.Categories = await _categoryService.GetCategories();
                    ViewBag.Suppliers = await _supplierService.GetSuppliers();
                }
                return View(product);
            }
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
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product != null)
                {
                    await _productService.RemoveProduct(product);
                }

                await _productService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}