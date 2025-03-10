using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using inventory.Models;
using inventory.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using inventory.Services.NotificationRepo; // Ensure the correct namespace for ProductDto

namespace inventory.Services.ProductRepo
{
    public class ProductService : IProductService
    {
        private readonly ProductContext _context;
        private readonly INotificationService _notificationService;

        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductContext context, INotificationService notificationService, ILogger<ProductService> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        // Add a new product to the database
        public async Task<bool> AddProduct(Product product)
        {
            try
            {
                await _context.Product.AddAsync(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error adding product");
                return false;
            }
        }

        // Get a product by its ID
        public async Task<Product?> GetProductByIdAsync(int? id)
        {
            try
            {
                return await _context.Product
                    .Include(p => p.Category)  // Include related Category
                    .Include(p => p.Supplier)  // Include related Supplier
                    .FirstOrDefaultAsync(p => p.ProductId == id); // Filter by ID
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error retrieving product by ID");
                return null;
            }
        }

        // Get all products
        public async Task<IEnumerable<Product>> GetProducts()
        {
            try
            {
                return await _context.Product.Include(p => p.Category).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error retrieving products");
                return Enumerable.Empty<Product>();
            }
        }

        // Check if a product exists by its ID
        public bool ProductExists(int id)
        {
            try
            {
                return _context.Product.Any(e => e.ProductId == id);
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error checking if product exists");
                return false;
            }
        }

        // Remove a product from the database
        public async Task<bool> RemoveProduct(Product product)
        {
            try
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error removing product");
                return false;
            }
        }

        // Save changes to the database
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error saving changes");
            }
        }

        // Update an existing product
        public async Task<bool> UpdateProduct(Product product)
        {
            try
            {
                _context.Product.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error updating product");
                return false;
            }
        }

        // Check and restock products that are below the reorder level
        public async Task CheckAndRestockProduct()
        {
            try
            {
                var products = await _context.Product.ToListAsync();
                foreach (var product in products)
                {
                    if (product.Quantity < 100) // Default reorder level
                    {
                        product.Quantity += 100;
                        _context.Product.Update(product);
                        var notification = new Notification()
                        {
                            Message = $"Product {product.Name} has been restocked to {product.Quantity}",
                            Date = System.DateTime.Now
                        };
                        await _notificationService.CreateNotificationAsync(notification);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error checking and restocking products");
            }
        }

        // Get products that are low in stock
        public async Task<IEnumerable<Product>> GetLowStockProducts()
        {
            try
            {
                return await _context.Product.Where(p => p.Quantity < 120).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error retrieving low stock products");
                return Enumerable.Empty<Product>();
            }
        }

        // Get the total count of products
        public async Task<int> GetProductCount()
        {
            try
            {
                return await _context.Product.CountAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error retrieving product count");
                return 0;
            }
        }
    }
}
