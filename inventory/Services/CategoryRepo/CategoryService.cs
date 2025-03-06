using inventory.Data;
using inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace inventory.Services.CategoryRepo;
    public class CategoryService:ICategoryService{
        private readonly ProductContext _context;
        public CategoryService(ProductContext context){

            _context = context;
        }
        public async Task<IEnumerable<Category>> GetCategories(){
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category?> GetCategoryByIdAsync(int? id){
            return await _context.Categories.FindAsync(id);
        }
        public async Task SaveChangesAsync(){
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateCategory(Category category){
            _context.Entry(category).State = EntityState.Modified;
            await SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddCategory(Category category){
            _context.Categories.Add(category);
            await SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveCategory(Category category){
            _context.Categories.Remove(category);
            await SaveChangesAsync();
            return true;
        }
        public bool CategoryExists(int id){
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
