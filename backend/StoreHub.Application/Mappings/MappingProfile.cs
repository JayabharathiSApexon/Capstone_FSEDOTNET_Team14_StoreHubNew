using AutoMapper;
using StoreHub.Application.Models.Category;
using StoreHub.Application.Models.Order;
using StoreHub.Application.Models.Product;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Category

            CreateMap<CategoryRequestModel, Category>();

            CreateMap<Category, CategoryResponseModel>();

            #endregion

            #region Product
            CreateMap<ProductRequestModel, Product>();
            CreateMap<ProductImageRequestModel, ProductImage>();
            CreateMap<ProductImage, ProductImageResponseModel>();
            CreateMap<Product, ProductResponseModel>()
                        .ForMember(d => d.CategoryName,
                            o => o.MapFrom(s => s.Category.Name))
                        .ForMember(d => d.Images,
                            o => o.MapFrom(s => s.ProductImages));
            #endregion

            #region Order
            CreateMap<Order, MyOrderResponseModel>();
            #endregion

        }
    }
}