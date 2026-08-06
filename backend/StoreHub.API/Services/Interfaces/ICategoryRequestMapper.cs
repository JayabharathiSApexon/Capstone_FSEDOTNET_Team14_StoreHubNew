using StoreHub.API.Models.Category;
using StoreHub.Application.Models.Category;

namespace StoreHub.API.Services.Interfaces
{
    public interface ICategoryRequestMapper
    {
        CategoryRequestModel ToApplicationModel(CategoryRequest request);
    }
}