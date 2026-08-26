using Microsoft.EntityFrameworkCore;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;
using StoreHub.Infrastructure.Repositories;

namespace StoreHub.Tests.Repositories;

public class RepositoryCoverageTests
{
    [Fact]
    public async Task Given_ActiveAndInactiveCategories_When_GetAllCategories_Then_ReturnsActiveSortedCategories()
    {
        await using var context = CreateContext();
        await context.Categories.AddRangeAsync(
            new Category { Id = Guid.NewGuid(), Name = "Zeta", IsActive = true },
            new Category { Id = Guid.NewGuid(), Name = "Alpha", IsActive = true },
            new Category { Id = Guid.NewGuid(), Name = "Hidden", IsActive = false });
        await context.SaveChangesAsync();

        var result = (await new CategoryRepository(context).GetAllCategoriesAsync()).ToList();

        Assert.Equal(new[] { "Alpha", "Zeta" }, result.Select(x => x.Name));
    }

    [Fact]
    public async Task Given_InactiveCategory_When_GetById_Then_ReturnsNull()
    {
        await using var context = CreateContext();
        var category = new Category { Id = Guid.NewGuid(), Name = "Hidden", IsActive = false };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        Assert.Null(await new CategoryRepository(context).GetCategoryByIdAsync(category.Id));
    }

    [Fact]
    public async Task Given_Category_When_CreateUpdateAndDelete_Then_PersistsChanges()
    {
        await using var context = CreateContext();
        var repository = new CategoryRepository(context);
        var category = new Category { Id = Guid.NewGuid(), Name = "Original", IsActive = true };

        await repository.CreateCategoryAsync(category);
        category.Name = "Updated";
        await repository.UpdateCategoryAsync(category);
        var deleted = await repository.DeleteCategoryAsync(category);

        Assert.Equal("Updated", (await context.Categories.FindAsync(category.Id))!.Name);
        Assert.False(deleted.IsActive);
        Assert.NotNull(deleted.UpdatedDate);
    }

    [Fact]
    public async Task Given_ActiveAndInactiveProducts_When_GetProducts_Then_ReturnsActiveWithCategoryAndImages()
    {
        await using var context = CreateContext();
        var category = new Category { Id = Guid.NewGuid(), Name = "Devices", IsActive = true };
        var product = new Product { Id = Guid.NewGuid(), CategoryId = category.Id, Category = category, Name = "Laptop", IsActive = true, ProductImages = new List<ProductImage>() };
        product.ProductImages.Add(new ProductImage { Id = Guid.NewGuid(), ProductId = product.Id, ImageUrl = "/laptop.png", IsActive = true });
        await context.Products.AddAsync(product);
        await context.Products.AddAsync(new Product { Id = Guid.NewGuid(), CategoryId = category.Id, Name = "Hidden", IsActive = false });
        await context.SaveChangesAsync();

        var result = (await new ProductRepository(context).GetAllProductsAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("Devices", result[0].Category.Name);
        Assert.Single(result[0].ProductImages);
    }

    [Fact]
    public async Task Given_InactiveProduct_When_GetById_Then_ReturnsNull()
    {
        await using var context = CreateContext();
        var product = new Product { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Name = "Hidden", IsActive = false };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        Assert.Null(await new ProductRepository(context).GetProductByIdAsync(product.Id));
    }

    [Fact]
    public async Task Given_Product_When_CreateUpdateDelete_Then_PersistsProductState()
    {
        await using var context = CreateContext();
        var category = new Category { Id = Guid.NewGuid(), Name = "Devices", IsActive = true };
        await context.Categories.AddAsync(category);
        var repository = new ProductRepository(context);
        var product = new Product { Id = Guid.NewGuid(), CategoryId = category.Id, Name = "Laptop", IsActive = true };

        await repository.CreateProductAsync(product);
        product.Name = "Updated Laptop";
        var updated = await repository.UpdateProductAsync(product);
        var deleted = await repository.DeleteProductAsync(product);

        Assert.Equal("Updated Laptop", updated.Name);
        Assert.False(deleted.IsActive);
        Assert.NotNull(deleted.UpdatedDate);
    }

    [Fact]
    public async Task Given_ExistingImages_When_ReplaceImages_Then_ReplacesAllImages()
    {
        await using var context = CreateContext();
        var product = new Product { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Name = "Laptop", IsActive = true };
        await context.Products.AddAsync(product);
        await context.ProductImages.AddAsync(new ProductImage { Id = Guid.NewGuid(), ProductId = product.Id, ImageUrl = "/old.png" });
        await context.SaveChangesAsync();
        var replacement = new List<ProductImage> { new() { Id = Guid.NewGuid(), ProductId = product.Id, ImageUrl = "/new.png" } };

        await new ProductRepository(context).ReplaceProductImagesAsync(product.Id, replacement);

        Assert.DoesNotContain(context.ProductImages, x => x.ImageUrl == "/old.png");
        Assert.Contains(context.ProductImages, x => x.ImageUrl == "/new.png");
    }

    [Fact]
    public async Task Given_NoExistingImages_When_ReplaceImages_Then_AddsImages()
    {
        await using var context = CreateContext();
        var productId = Guid.NewGuid();
        var replacement = new List<ProductImage> { new() { Id = Guid.NewGuid(), ProductId = productId, ImageUrl = "/new.png" } };

        await new ProductRepository(context).ReplaceProductImagesAsync(productId, replacement);

        Assert.Single(context.ProductImages);
    }

    [Fact]
    public async Task Given_CartWithItems_When_GetCart_Then_LoadsProductImages()
    {
        await using var context = CreateContext();
        var product = new Product { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Name = "Mouse", IsActive = true, ProductImages = new List<ProductImage>() };
        product.ProductImages.Add(new ProductImage { Id = Guid.NewGuid(), ProductId = product.Id, ImageUrl = "/mouse.png" });
        var cart = new Cart { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), CartItems = new List<CartItem> { new() { Id = Guid.NewGuid(), ProductId = product.Id, Product = product, Quantity = 2 } } };
        await context.Products.AddAsync(product);
        await context.Carts.AddAsync(cart);
        await context.SaveChangesAsync();

        var result = await new CartRepository(context).GetCartByUserIdAsync(cart.UserId);

        Assert.NotNull(result);
        Assert.Equal("Mouse", result!.CartItems.First().Product.Name);
        Assert.Single(result.CartItems.First().Product.ProductImages);
    }

    [Fact]
    public async Task Given_Cart_When_CreateUpdateAddDeleteAndClear_Then_PersistsOperations()
    {
        await using var context = CreateContext();
        var repository = new CartRepository(context);
        var cart = new Cart { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
        await repository.CreateCartAsync(cart);
        cart.UpdatedDate = DateTime.UtcNow;
        await repository.UpdateCartAsync(cart);
        var item = new CartItem { Id = Guid.NewGuid(), CartId = cart.Id, ProductId = Guid.NewGuid(), Quantity = 1 };
        await repository.AddCartItemAsync(item);
        item.Quantity = 3;
        await repository.UpdateCartItemAsync(item);
        Assert.NotNull(await repository.GetCartItemAsync(cart.Id, item.ProductId));
        await repository.DeleteCartItemAsync(item.Id);
        await repository.ClearCartAsync(cart.Id);

        Assert.Empty(context.CartItems);
    }

    [Fact]
    public async Task Given_MissingCartItemOrEmptyCart_When_DeleteAndClear_Then_DoesNothing()
    {
        await using var context = CreateContext();
        var repository = new CartRepository(context);

        await repository.DeleteCartItemAsync(Guid.NewGuid());
        await repository.ClearCartAsync(Guid.NewGuid());

        Assert.Empty(context.CartItems);
    }

    [Fact]
    public async Task Given_Orders_When_Querying_Then_ReturnsNewestAndIncludesTracking()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        var older = new Order { Id = Guid.NewGuid(), UserId = userId, OrderDate = DateTime.UtcNow.AddDays(-1), Status = "Pending" };
        var newer = new Order { Id = Guid.NewGuid(), UserId = userId, OrderDate = DateTime.UtcNow, Status = "Processing", TrackingHistory = new List<OrderTrackingHistory> { new() { Id = Guid.NewGuid(), Status = "Processing", StatusDate = DateTime.UtcNow } } };
        await context.Orders.AddRangeAsync(older, newer);
        await context.SaveChangesAsync();
        var repository = new OrderRepository(context);

        var userOrders = (await repository.GetOrdersByUserIdAsync(userId)).ToList();
        var loaded = await repository.GetOrderByIdAsync(newer.Id);
        var allOrders = (await repository.GetAllOrdersAsync()).ToList();

        Assert.Equal(newer.Id, userOrders[0].Id);
        Assert.Single(loaded!.TrackingHistory);
        Assert.Equal(newer.Id, allOrders[0].Id);
    }

    [Fact]
    public async Task Given_Order_When_SavingStatusChangeAndChanges_Then_PersistsHistory()
    {
        await using var context = CreateContext();
        var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = "Pending" };
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
        var history = new OrderTrackingHistory { Id = Guid.NewGuid(), OrderId = order.Id, Status = "Processing", StatusDate = DateTime.UtcNow };
        var repository = new OrderRepository(context);

        await repository.AddOrderAsync(new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = "Pending" });
        await repository.SaveOrderStatusChangeAsync(order, history);
        await repository.SaveChangesAsync();

        Assert.NotNull(await context.OrderTrackingHistory.FindAsync(history.Id));
    }

    [Fact]
    public async Task Given_TrackedOrder_When_GetTrackingDetails_Then_LoadsItemsImagesAndHistory()
    {
        await using var context = CreateContext();
        var product = new Product { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Name = "Keyboard", IsActive = true, ProductImages = new List<ProductImage>() };
        product.ProductImages.Add(new ProductImage { Id = Guid.NewGuid(), ProductId = product.Id, ImageUrl = "/keyboard.png" });
        var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = "Pending", OrderItems = new List<OrderItem> { new() { Id = Guid.NewGuid(), ProductId = product.Id, Product = product, Quantity = 1 } }, TrackingHistory = new List<OrderTrackingHistory> { new() { Id = Guid.NewGuid(), Status = "Pending", StatusDate = DateTime.UtcNow } } };
        await context.Products.AddAsync(product);
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var result = await new TrackingDetails(context).GetTrackingDetailsAsync(order.Id);

        Assert.NotNull(result);
        Assert.Equal("Keyboard", result!.OrderItems.First().Product.Name);
        Assert.Single(result.TrackingHistory);
    }

    [Fact]
    public async Task Given_MissingTrackingOrder_When_GetTrackingDetails_Then_ReturnsNull()
    {
        await using var context = CreateContext();

        Assert.Null(await new TrackingDetails(context).GetTrackingDetailsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Given_Users_When_Querying_Then_FiltersAndOrdersCorrectly()
    {
        await using var context = CreateContext();
        var active = new User { Id = Guid.NewGuid(), FullName = "Alpha User", Email = "ALPHA@TEST.COM", IsActive = true, CreatedDate = DateTime.UtcNow };
        var inactive = new User { Id = Guid.NewGuid(), FullName = "Inactive", Email = "inactive@test.com", IsActive = false, CreatedDate = DateTime.UtcNow.AddDays(1) };
        await context.Users.AddRangeAsync(active, inactive);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context);

        Assert.True(await repository.ExistsActiveByEmailAsync("alpha@test.com"));
        Assert.NotNull(await repository.GetActiveByEmailOrFullNameAsync("alpha user"));
        Assert.NotNull(await repository.GetActiveByEmailAsync("alpha@test.com"));
        Assert.NotNull(await repository.GetByEmailAsync("alpha@test.com"));
        var allUsers = await repository.GetAllUsersOrderedByCreatedDateDescAsync();
        Assert.Equal(new[] { inactive.Id, active.Id }, allUsers.Select(user => user.Id));
        Assert.NotNull(await repository.GetActiveUserByIdAsync(active.Id));
    }

    [Fact]
    public async Task Given_MissingOrInactiveUser_When_Querying_Then_ReturnsFalseOrNull()
    {
        await using var context = CreateContext();
        var inactive = new User { Id = Guid.NewGuid(), FullName = "Inactive", Email = "inactive@test.com", IsActive = false };
        await context.Users.AddAsync(inactive);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context);

        Assert.False(await repository.ExistsActiveByEmailAsync("missing@test.com"));
        Assert.Null(await repository.GetActiveByEmailOrFullNameAsync("inactive"));
        Assert.Null(await repository.GetActiveByEmailAsync("inactive@test.com"));
        Assert.Null(await repository.GetActiveUserByIdAsync(inactive.Id));
        Assert.Null(await repository.GetByEmailAsync("missing@test.com"));
    }

    [Fact]
    public async Task Given_NewUser_When_AddAndSave_Then_PersistsUser()
    {
        await using var context = CreateContext();
        var user = new User { Id = Guid.NewGuid(), FullName = "New User", Email = "new@test.com", IsActive = true };
        var repository = new UserRepository(context);

        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        Assert.NotNull(await context.Users.FindAsync(user.Id));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
