using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using StoreHub.Application.Exceptions;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Category;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryQueryRepository _categoryQueryRepository;
        private readonly ICategoryCommandRepository _categoryCommandRepository;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryQueryRepository categoryQueryRepository,
            ICategoryCommandRepository categoryCommandRepository,
            IMapper mapper)
        {
            _categoryQueryRepository = categoryQueryRepository;
            _categoryCommandRepository = categoryCommandRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseModel>> GetAllCategoriesAsync()
        {
            var categories = await _categoryQueryRepository.GetAllCategoriesAsync();

            return _mapper.Map<IEnumerable<CategoryResponseModel>>(categories);
        }

        public async Task<CategoryResponseModel?> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _categoryQueryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null)
                return null;

            return _mapper.Map<CategoryResponseModel>(category);
        }

        public async Task<CategoryResponseModel> CreateCategoryAsync(CategoryRequestModel request)
        {
            var category = _mapper.Map<Category>(request);

            category.Id = Guid.NewGuid();
            category.CreatedDate = DateTime.UtcNow;

            var createdCategory = await _categoryCommandRepository.CreateCategoryAsync(category);

            return _mapper.Map<CategoryResponseModel>(createdCategory);
        }

        public async Task<CategoryResponseModel> UpdateCategoryAsync(CategoryRequestModel request)
        {
            var category = await _categoryQueryRepository.GetCategoryByIdAsync(request.Id);

            if (category == null)
                throw new NotFoundException("Category not found.");

            _mapper.Map(request, category);

            category.UpdatedDate = DateTime.UtcNow;

            var updatedCategory = await _categoryCommandRepository.UpdateCategoryAsync(category);

            return _mapper.Map<CategoryResponseModel>(updatedCategory);
        }

        public async Task<CategoryResponseModel> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _categoryQueryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null)
                throw new NotFoundException("Category not found.");

            var deletedCategory = await _categoryCommandRepository.DeleteCategoryAsync(category);

            return _mapper.Map<CategoryResponseModel>(deletedCategory);
        }
    }
}