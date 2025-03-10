using inventory.Data;
using inventory.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace inventory.Services.SupplierRepo
{
    public class SupplierService : ISupplierService
    {
        private readonly ProductContext _context;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(ProductContext context, ILogger<SupplierService> logger)
        {
            _context = context;
            _logger = logger;
        }   
        
        // Get all suppliers
        public async Task<IEnumerable<Supplier>> GetSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        // Get supplier by ID
        public async Task<Supplier?> GetSupplierByIdAsync(int? id)
        {
            try
            {
                return await _context.Suppliers.Include(s => s.Products).FirstOrDefaultAsync(s => s.SupplierId == id);
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                _logger.LogError(ex, "Error retrieving supplier by ID");
                return null;
            }
        }

        // Save changes to the database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Update supplier details
        public async Task<bool> UpdateSupplier(Supplier supplier)
        {
            try
            {
                _context.Entry(supplier).State = EntityState.Modified;
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                _logger.LogError(ex, "Error updating supplier");
                return false;
            }
        }

        // Add a new supplier
        public async Task<bool> AddSupplier(Supplier supplier)
        {
            try
            {
                _context.Suppliers.Add(supplier);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                _logger.LogError(ex, "Error adding supplier");
                return false;
            }
        }

        // Remove a supplier
        public async Task<bool> RemoveSupplier(Supplier supplier)
        {
            try
            {
                _context.Suppliers.Remove(supplier);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                _logger.LogError(ex, "Error removing supplier");
                return false;
            }
        }

        // Check if a supplier exists by ID
        public bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }

        // Get top 5 suppliers based on the number of products
        public async Task<IEnumerable<Supplier>> GetTopSuppliers()
        {
            try
            {
                return await _context.Suppliers.OrderByDescending(s => s.Products.Count).Take(5).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                _logger.LogError(ex, "Error retrieving top suppliers");
                return new List<Supplier>();
            }
        }
    }
}
