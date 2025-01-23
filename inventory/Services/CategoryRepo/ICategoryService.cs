using inventory.Models;

namespace inventory.Services.CategoryRepo;
    public interface ICategoryService{
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategoryByIdAsync(int? id);
        Task SaveChangesAsync();
        Task<bool> UpdateCategory(Category category);
        Task<bool> AddCategory(Category category);
        Task<bool> RemoveCategory(Category category);
        bool CategoryExists(int id);
    }
