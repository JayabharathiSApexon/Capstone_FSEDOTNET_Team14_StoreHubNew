using StoreHub.API.Models.Product;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Services.Interfaces
{
    public interface IProductRequestMapper
    {
        ProductRequestModel ToCreateModel(CreateProductRequest request, List<ProductImageRequestModel> images);

        ProductRequestModel ToUpdateModel(UpdateProductRequest request, List<ProductImageRequestModel> images);
    }
}