using Microsoft.Extensions.DependencyInjection;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Services;
using StoreHub.Infrastructure.Repositories;
using StoreHub.Application.Mappings;
public static class ServiceCollectionExtensions
{
     public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITrackingDetails, TrackingDetails>();

        // Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ITrackingService, TrackingService>();

        return services;
    }
}