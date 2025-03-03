using System;
using inventory.Models;

namespace inventory.Services.ProductRepo;

public interface IProductService{

    Task<IEnumerable<Product>> GetProducts();

    Task<Product?> GetProductByIdAsync(int? id);

    Task SaveChangesAsync();

    Task<bool> UpdateProduct(Product product);

    Task<bool> AddProduct(Product product);

    Task<bool> RemoveProduct(Product product);

    Task<IEnumerable<Product>> GetLowStockProducts();

    Task<int> GetProductCount();

    Task CheckAndRestockProduct();

    bool ProductExists(int id);
    



    
}