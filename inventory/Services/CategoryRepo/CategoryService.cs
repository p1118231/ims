using inventory.Data;
using inventory.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inventory.Services.CategoryRepo
{
    public class CategoryService : ICategoryService
    {
        private readonly ProductContext _context;

        public CategoryService(ProductContext context)
        {
            _context = context;
        }

        // Get all categories
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // Get category by ID
        public async Task<Category?> GetCategoryByIdAsync(int? id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // Save changes to the database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Update an existing category
        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                _context.Entry(category).State = EntityState.Modified;
                await SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Log exception or handle it as needed
                return false;
            }
        }

        // Add a new category
        public async Task<bool> AddCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Log exception or handle it as needed
                return false;
            }
        }

        // Remove an existing category
        public async Task<bool> RemoveCategory(Category category)
        {
            try
            {
                _context.Categories.Remove(category);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Log exception or handle it as needed
                return false;
            }
        }

        // Check if a category exists by ID
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
