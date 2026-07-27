using AutoMapper;
using StoreHub.API.Models.Product;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Mappings
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<CreateProductRequest, ProductRequestModel>();
            CreateMap<UpdateProductRequest, ProductRequestModel>();
        }
    }
}