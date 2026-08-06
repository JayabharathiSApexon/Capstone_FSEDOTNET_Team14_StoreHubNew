using AutoMapper;
using StoreHub.API.Models.Category;
using StoreHub.API.Services.Interfaces;
using StoreHub.Application.Models.Category;

namespace StoreHub.API.Services
{
    public class CategoryRequestMapper : ICategoryRequestMapper
    {
        private readonly IMapper _mapper;

        public CategoryRequestMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public CategoryRequestModel ToApplicationModel(CategoryRequest request)
        {
            return _mapper.Map<CategoryRequestModel>(request);
        }
    }
}