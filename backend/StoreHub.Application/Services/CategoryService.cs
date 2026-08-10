using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Category;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseModel>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            return _mapper.Map<IEnumerable<CategoryResponseModel>>(categories);
        }

        public async Task<CategoryResponseModel?> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null)
                return null;

            return _mapper.Map<CategoryResponseModel>(category);
        }

        public async Task<CategoryResponseModel> CreateCategoryAsync(CategoryRequestModel request)
        {
            var category = _mapper.Map<Category>(request);

            category.Id = Guid.NewGuid();
            category.CreatedDate = DateTime.UtcNow;

            var createdCategory = await _categoryRepository.CreateCategoryAsync(category);

            return _mapper.Map<CategoryResponseModel>(createdCategory);
        }

        public async Task<CategoryResponseModel> UpdateCategoryAsync(CategoryRequestModel request)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);

            if (category == null)
                throw new Exception("Category not found.");

            _mapper.Map(request, category);

            category.UpdatedDate = DateTime.UtcNow;

            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);

            return _mapper.Map<CategoryResponseModel>(updatedCategory);
        }

        public async Task<CategoryResponseModel> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null)
                throw new Exception("Category not found.");

            var deletedCategory = await _categoryRepository.DeleteCategoryAsync(category);

            return _mapper.Map<CategoryResponseModel>(deletedCategory);
        }
    }
}