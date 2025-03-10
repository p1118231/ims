using inventory.Models;
using inventory.Services.SupplierRepo;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers
{
    public class SupplierController : Controller
    {
        // Dependency injection for supplier service and logger
        public readonly ISupplierService _supplierService;
        public readonly ILogger<SupplierController> _logger;

        // Constructor to initialize the dependencies
        public SupplierController(ISupplierService supplierService, ILogger<SupplierController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        // Action method to display the list of suppliers
        public async Task<IActionResult> Index(string? query)
        {
            IEnumerable<Supplier> suppliers = null!;
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    var allSuppliers = await _supplierService.GetSuppliers();
                    suppliers = allSuppliers?.Where(s => (s.Name ?? "").Contains(query, StringComparison.OrdinalIgnoreCase)) ?? Enumerable.Empty<Supplier>();
                }
                else
                {
                    suppliers = await _supplierService.GetSuppliers();
                }
                ViewData["SearchQuery"] = query;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failure to access supplier service: {ex.Message}");
                suppliers = Array.Empty<Supplier>();
            }
            return View(suppliers);
        }

        // Action method to display the create supplier form
        public IActionResult Create()
        {
            return View();
        }

        // Action method to handle the creation of a new supplier
        [HttpPost]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            try
            {
                await _supplierService.AddSupplier(supplier);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failure to add supplier: {ex.Message}");
                return View(supplier);
            }
        }

        // Action method to display the edit supplier form
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (!_supplierService.SupplierExists(id))
                {
                    return NotFound();
                }
                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                return View(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failure to access supplier service: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // Action method to handle the editing of an existing supplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,Name,Address,Phone,Email")] Supplier supplier)
        {
            try
            {
                if (id != supplier.SupplierId)
                {
                    return NotFound();
                }
                await _supplierService.UpdateSupplier(supplier);
                await _supplierService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failure to update supplier: {ex.Message}");
                return View(supplier);
            }
        }

        // Action method to display the details of a supplier
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                return View(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failure to access supplier service: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}