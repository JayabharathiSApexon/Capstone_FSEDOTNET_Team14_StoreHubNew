using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

internal sealed class TestCartRepository : ICartRepository
{
    public Cart? Cart { get; set; }
    public CartItem? CartItem { get; set; }
    public Guid? DeletedItemId { get; private set; }
    public Guid? ClearedCartId { get; private set; }
    public Task<Cart?> GetCartByUserIdAsync(Guid userId) => Task.FromResult(Cart?.UserId == userId ? Cart : null);
    public Task<Cart> CreateCartAsync(Cart cart) { Cart = cart; return Task.FromResult(cart); }
    public Task<Cart> UpdateCartAsync(Cart cart) => Task.FromResult(cart);
    public Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId) => Task.FromResult(CartItem);
    public Task AddCartItemAsync(CartItem cartItem) { CartItem = cartItem; Cart!.CartItems.Add(cartItem); return Task.CompletedTask; }
    public Task UpdateCartItemAsync(CartItem cartItem) { CartItem = cartItem; return Task.CompletedTask; }
    public Task DeleteCartItemAsync(Guid cartItemId) { DeletedItemId = cartItemId; return Task.CompletedTask; }
    public Task ClearCartAsync(Guid cartId) { ClearedCartId = cartId; return Task.CompletedTask; }
}

internal sealed class TestCategoryRepository : ICategoryRepository
{
    public List<Category> Categories { get; } = new();
    public Task<IEnumerable<Category>> GetAllCategoriesAsync() => Task.FromResult<IEnumerable<Category>>(Categories);
    public Task<Category?> GetCategoryByIdAsync(Guid categoryId) => Task.FromResult(Categories.FirstOrDefault(x => x.Id == categoryId));
    public Task<Category> CreateCategoryAsync(Category category) { Categories.Add(category); return Task.FromResult(category); }
    public Task<Category> UpdateCategoryAsync(Category category) => Task.FromResult(category);
    public Task<Category> DeleteCategoryAsync(Category category) { category.IsActive = false; return Task.FromResult(category); }
}

internal sealed class TestProductRepository : IProductRepository
{
    public List<Product> Products { get; } = new();
    public List<ProductImage> ReplacedImages { get; private set; } = new();
    public Task<IEnumerable<Product>> GetAllProductsAsync() => Task.FromResult<IEnumerable<Product>>(Products);
    public Task<Product?> GetProductByIdAsync(Guid productId) => Task.FromResult(Products.FirstOrDefault(x => x.Id == productId));
    public Task<Product> CreateProductAsync(Product product) { Products.Add(product); return Task.FromResult(product); }
    public Task<Product> UpdateProductAsync(Product product) => Task.FromResult(product);
    public Task<Product> DeleteProductAsync(Product product) { product.IsActive = false; return Task.FromResult(product); }
    public Task ReplaceProductImagesAsync(Guid productId, List<ProductImage> newImages) { ReplacedImages = newImages; return Task.CompletedTask; }
}

internal sealed class TestUserRepository : IUserRepository
{
    public User? User { get; set; }
    public User? ExistingUser { get; set; }
    public int SaveChangesCalls { get; private set; }
    public Task<bool> ExistsActiveByEmailAsync(string normalizedEmail) => Task.FromResult(false);
    public Task<User?> GetActiveByEmailOrFullNameAsync(string normalizedIdentifier) => Task.FromResult<User?>(null);
    public Task<User?> GetActiveByEmailAsync(string normalizedEmail) => Task.FromResult<User?>(null);
    public Task<User?> GetByEmailAsync(string normalizedEmail) => Task.FromResult(ExistingUser);
    public Task<IReadOnlyList<User>> GetAllUsersOrderedByCreatedDateDescAsync() => Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());
    public Task<User?> GetActiveUserByIdAsync(Guid userId) => Task.FromResult(User?.Id == userId && User.IsActive ? User : null);
    public Task AddAsync(User user) => Task.CompletedTask;
    public Task SaveChangesAsync() { SaveChangesCalls++; return Task.CompletedTask; }
}

internal sealed class TestTrackingRepository : ITrackingDetails
{
    public Order? Order { get; set; }
    public Task<Order?> GetTrackingDetailsAsync(Guid orderId) => Task.FromResult(Order?.Id == orderId ? Order : null);
}

internal sealed class TestOrderRepository : IOrderRepository
{
    public List<Order> Orders { get; } = new();
    public OrderTrackingHistory? SavedHistory { get; private set; }
    public Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId) => Task.FromResult<IEnumerable<Order>>(Orders.Where(x => x.UserId == userId));
    public Task<Order?> GetOrderByIdAsync(Guid orderId) => Task.FromResult(Orders.FirstOrDefault(x => x.Id == orderId));
    public Task<IEnumerable<Order>> GetAllOrdersAsync() => Task.FromResult<IEnumerable<Order>>(Orders);
    public Task SaveOrderStatusChangeAsync(Order order, OrderTrackingHistory trackingHistory) { SavedHistory = trackingHistory; return Task.CompletedTask; }
    public Task<Order> AddOrderAsync(Order order) { Orders.Add(order); return Task.FromResult(order); }
    public Task SaveChangesAsync() => Task.CompletedTask;
}

internal sealed class RecordingInventoryService : StoreHub.Application.Interfaces.Services.IInventoryService
{
    public List<(Guid ProductId, int Quantity)> Reservations { get; } = new();
    public IEnumerable<OrderItem>? RestoredItems { get; private set; }
    public bool ReservationResult { get; set; } = true;
    public Task<bool> ReserveStockAsync(Guid productId, int quantity) { Reservations.Add((productId, quantity)); return Task.FromResult(ReservationResult); }
    public Task ReleaseStockAsync(Guid productId, int quantity) => Task.CompletedTask;
    public Task RestoreStockAsync(IEnumerable<OrderItem> orderItems) { RestoredItems = orderItems; return Task.CompletedTask; }
    public Task<int> GetStockQuantityAsync(Guid productId) => Task.FromResult(0);
}