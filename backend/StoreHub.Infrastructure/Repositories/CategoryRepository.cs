using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;
using StoreHub.Application.Interfaces.Repositories;

namespace StoreHub.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryQueryRepository, ICategoryCommandRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(category => category.IsActive)
                .OrderBy(category => category.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _context.Categories.FirstOrDefaultAsync(category => category.Id == categoryId && category.IsActive);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> DeleteCategoryAsync(Category category)
        {
            category.IsActive = false;
            category.UpdatedDate = DateTime.UtcNow;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return category;
        }
    }
}