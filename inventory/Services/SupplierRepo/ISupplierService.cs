using inventory.Models;

namespace inventory.Services.SupplierRepo;
    public interface ISupplierService{
        Task<IEnumerable<Supplier>> GetSuppliers();
        Task<Supplier?> GetSupplierByIdAsync(int? id);
        Task SaveChangesAsync();
        Task<bool> UpdateSupplier(Supplier supplier);
        Task<bool> AddSupplier(Supplier supplier);
        Task<bool> RemoveSupplier(Supplier supplier);
        bool SupplierExists(int id);

        Task<IEnumerable<Supplier>> GetTopSuppliers();

    
    }
