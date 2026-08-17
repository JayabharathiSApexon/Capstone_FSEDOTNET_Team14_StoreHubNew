using Microsoft.Extensions.DependencyInjection;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Services;
using StoreHub.API.Services;
using StoreHub.API.Services.Interfaces;
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
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITrackingDetails, TrackingDetails>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ITrackingService, TrackingService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IProfileService, ProfileService>();
        
        // SOLID Principle Implementation - Dependency Inversion
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IOrderStatusValidator, OrderStatusValidator>();

        // API services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IAuthTokenFactory, JwtAuthTokenFactory>();
        services.AddScoped<IProductRequestMapper, ProductRequestMapper>();
        services.AddScoped<ICategoryRequestMapper, CategoryRequestMapper>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddScoped<IProductImageStorageService, ProductImageStorageService>();
        services.AddScoped<IAdminSeedService, AdminSeedService>();

        return services;
    }
}