using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using inventory.Models;
using inventory.Data;
using Microsoft.EntityFrameworkCore; // Ensure the correct namespace for ProductDto

namespace inventory.Services.ProductRepo
{
    public class ProductService : IProductService
    {

        private readonly ProductContext _context;

            public ProductService(ProductContext context)
        {
            _context = context;
        }

        public async Task<bool> AddProduct(Product product)
        {
             // Save changes
        await _context.Product.AddAsync(product);
        await _context.SaveChangesAsync();
        return true;

        }

        public async Task<Product?> GetProductByIdAsync(int? id)
        {
            return await _context.Product
            .Include(p => p.Category)  // Include related Category
            .Include(p => p.Supplier)  // Include related Supplier
            .FirstOrDefaultAsync(p => p.ProductId == id); // Filter by ID

        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return (IEnumerable<Product>)await _context.Product.Include(p=> p.Category).ToListAsync();
        }

        public bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }

        public async Task<bool> RemoveProduct(Product product)
        {
        _context.Product.Remove(product);
        await _context.SaveChangesAsync();
        return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            // Save changes
         _context.Product.Update(product);
        await _context.SaveChangesAsync();
        return true;
        }
    }
}
