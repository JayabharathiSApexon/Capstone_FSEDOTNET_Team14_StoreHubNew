using AutoMapper;
using StoreHub.API.Models.Product;
using StoreHub.API.Services.Interfaces;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Services
{
    public class ProductRequestMapper : IProductRequestMapper
    {
        private readonly IMapper _mapper;

        public ProductRequestMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ProductRequestModel ToCreateModel(CreateProductRequest request, List<ProductImageRequestModel> images)
        {
            var model = _mapper.Map<ProductRequestModel>(request);
            model.Images = images;
            return model;
        }

        public ProductRequestModel ToUpdateModel(UpdateProductRequest request, List<ProductImageRequestModel> images)
        {
            var model = _mapper.Map<ProductRequestModel>(request);
            model.Images = images;
            return model;
        }
    }
}