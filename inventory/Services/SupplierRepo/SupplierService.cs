using inventory.Data;
using inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace inventory.Services.SupplierRepo;

    public class SupplierService:ISupplierService{
        private readonly ProductContext _context;
        public SupplierService(ProductContext context){
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetSuppliers(){
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int? id){
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task SaveChangesAsync(){
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateSupplier(Supplier supplier){
            _context.Entry(supplier).State = EntityState.Modified;
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddSupplier(Supplier supplier){
            _context.Suppliers.Add(supplier);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSupplier(Supplier supplier){
            _context.Suppliers.Remove(supplier);
            await SaveChangesAsync();
            return true;
        }

        public bool SupplierExists(int id){
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }

