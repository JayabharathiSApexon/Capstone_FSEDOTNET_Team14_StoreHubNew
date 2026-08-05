using Microsoft.Extensions.DependencyInjection;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Services;
using StoreHub.Infrastructure.Repositories;
using StoreHub.Application.Mappings;
using StoreHub.API.Common.Services;
public static class ServiceCollectionExtensions
{
     public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Services
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}