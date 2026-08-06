using AutoMapper;
using StoreHub.API.Models.Category;
using StoreHub.API.Models.Product;
using StoreHub.Application.Models.Category;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Mappings
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<CategoryRequest, CategoryRequestModel>();
            CreateMap<CreateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<UpdateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore());
        }
    }
}