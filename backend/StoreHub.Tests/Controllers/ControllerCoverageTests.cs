using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreHub.API.Controllers;
using StoreHub.API.Models.Category;
using StoreHub.API.Models.User;
using StoreHub.API.Services.Interfaces;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Cart;
using StoreHub.Application.Models.Category;
using StoreHub.Application.Models.Order;
using StoreHub.Application.Models.Profile;
using StoreHub.Domain.Enums;

namespace StoreHub.Tests.Controllers;

public class ControllerCoverageTests
{
    [Fact]
    public async Task Given_Categories_When_GetAllCategories_Then_ReturnsOk()
    {
        var result = await new CategoryController(new FakeCategoryService(), new FakeCategoryMapper()).GetAllCategories();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Given_MissingCategory_When_GetCategoryById_Then_ReturnsNotFound()
    {
        var result = await new CategoryController(new FakeCategoryService(), new FakeCategoryMapper()).GetCategoryById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Given_CategoryRequest_When_CreateCategory_Then_ReturnsCreated()
    {
        var result = await new CategoryController(new FakeCategoryService(), new FakeCategoryMapper()).CreateCategory(new CategoryRequest { Name = "Audio" });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CategoryController.GetCategoryById), created.ActionName);
    }

    [Fact]
    public async Task Given_CategoryRequest_When_UpdateCategory_Then_ReturnsOk()
    {
        var result = await new CategoryController(new FakeCategoryService(), new FakeCategoryMapper()).UpdateCategory(new CategoryRequest { Name = "Audio" });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Given_CategoryId_When_DeleteCategory_Then_ReturnsOk()
    {
        var result = await new CategoryController(new FakeCategoryService(), new FakeCategoryMapper()).DeleteCategory(Guid.NewGuid());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Given_AuthenticatedUser_When_GetCart_Then_ReturnsOk()
    {
        var controller = WithUser(new CartController(new FakeCartService()), Guid.NewGuid());

        Assert.IsType<OkObjectResult>(await controller.GetCart());
    }

    [Fact]
    public async Task Given_MissingClaim_When_GetCart_Then_ReturnsBadRequest()
    {
        var result = await WithClaim(new CartController(new FakeCartService()), "unused", "unused").GetCart();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public async Task Given_MissingClaim_When_AddToCart_Then_ReturnsBadRequest()
    {
        var result = await new CartController(new FakeCartService()).AddToCart(new AddToCartRequestModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Given_MismatchedCartItemId_When_UpdateCartItem_Then_ReturnsBadRequest()
    {
        var request = new UpdateCartRequestModel { CartItemId = Guid.NewGuid() };
        var result = await WithUser(new CartController(new FakeCartService()), Guid.NewGuid()).UpdateCartItem(Guid.NewGuid(), request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cart Item ID mismatch.", badRequest.Value);
    }

    [Fact]
    public async Task Given_AuthenticatedUser_When_CartOperationsSucceed_Then_ReturnsExpectedResponses()
    {
        var controller = WithUser(new CartController(new FakeCartService()), Guid.NewGuid());
        var cartItemId = Guid.NewGuid();

        Assert.IsType<OkObjectResult>(await controller.AddToCart(new AddToCartRequestModel()));
        Assert.IsType<OkObjectResult>(await controller.UpdateCartItem(cartItemId, new UpdateCartRequestModel { CartItemId = cartItemId }));
        Assert.IsType<OkResult>(await controller.RemoveCartItem(cartItemId));
        Assert.IsType<OkResult>(await controller.ClearCart());
    }

    [Fact]
    public async Task Given_ValidProfile_When_GetProfile_Then_ReturnsOk()
    {
        var controller = WithUser(new ProfileController(new FakeProfileService()), Guid.NewGuid());

        Assert.IsType<OkObjectResult>(await controller.GetProfile());
    }

    [Fact]
    public async Task Given_MissingProfile_When_GetProfile_Then_ReturnsNotFound()
    {
        var controller = WithUser(new ProfileController(new FakeProfileService { ReturnNull = true }), Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(await controller.GetProfile());
    }

    [Fact]
    public async Task Given_InvalidClaim_When_GetProfile_Then_ReturnsUnauthorized()
    {
        var controller = WithClaim(new ProfileController(new FakeProfileService()), ClaimTypes.NameIdentifier, "invalid-guid");

        Assert.IsType<UnauthorizedResult>(await controller.GetProfile());
    }

    [Fact]
    public async Task Given_InvalidModel_When_UpdateProfile_Then_ReturnsValidationProblem()
    {
        var controller = WithUser(new ProfileController(new FakeProfileService()), Guid.NewGuid());
        controller.ModelState.AddModelError("Email", "Email is required");

        Assert.IsType<ObjectResult>(await controller.UpdateProfile(new UpdateProfileRequest()));
    }

    [Theory]
    [InlineData(UpdateProfileResult.Success, typeof(OkObjectResult))]
    [InlineData(UpdateProfileResult.UserNotFound, typeof(NotFoundObjectResult))]
    [InlineData(UpdateProfileResult.EmailAlreadyExists, typeof(ConflictObjectResult))]
    public async Task Given_ProfileUpdateResult_When_UpdateProfile_Then_ReturnsMappedResponse(UpdateProfileResult updateResult, Type expectedType)
    {
        var controller = WithUser(new ProfileController(new FakeProfileService { UpdateResult = updateResult }), Guid.NewGuid());

        var result = await controller.UpdateProfile(new UpdateProfileRequest { FullName = "User", Email = "user@test.com" });

        Assert.IsType(expectedType, result);
    }

    [Fact]
    public async Task Given_InvalidClaim_When_UpdateProfile_Then_ReturnsUnauthorized()
    {
        var controller = WithClaim(new ProfileController(new FakeProfileService()), ClaimTypes.NameIdentifier, "invalid-guid");

        Assert.IsType<UnauthorizedResult>(await controller.UpdateProfile(new UpdateProfileRequest()));
    }

    [Fact]
    public async Task Given_Users_When_GetAllUsers_Then_ReturnsOk()
    {
        var result = await new UsersController(new FakeUserQueryService()).GetAllUsers();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Given_Orders_When_GetMyOrdersAndTracking_Then_ReturnsOk()
    {
        var controller = new OrderController(new FakeOrderService(), new FakeTrackingService());

        Assert.IsType<OkObjectResult>(await controller.GetMyOrders(Guid.NewGuid()));
        Assert.IsType<OkObjectResult>(await controller.GetTracking(Guid.NewGuid()));
    }

    [Fact]
    public async Task Given_MissingTracking_When_GetTracking_Then_ReturnsNotFound()
    {
        var result = await new OrderController(new FakeOrderService(), new FakeTrackingService { ReturnNull = true }).GetTracking(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Given_MissingClaim_When_GetMyOrdersFromClaims_Then_ReturnsUnauthorized()
    {
        var result = await WithClaim(new OrderController(new FakeOrderService(), new FakeTrackingService()), "unused", "unused").GetMyOrdersFromClaims();

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Given_ValidClaim_When_GetMyOrdersFromClaims_Then_ReturnsOk()
    {
        var controller = WithUser(new OrderController(new FakeOrderService(), new FakeTrackingService()), Guid.NewGuid());

        Assert.IsType<OkObjectResult>(await controller.GetMyOrdersFromClaims());
    }

    [Fact]
    public async Task Given_Orders_When_GetAllOrders_Then_ReturnsOk()
    {
        var result = await new OrderController(new FakeOrderService(), new FakeTrackingService()).GetAllOrders();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Given_MismatchedOrderId_When_UpdateOrderStatus_Then_ReturnsBadRequest()
    {
        var result = await new OrderController(new FakeOrderService(), new FakeTrackingService()).UpdateOrderStatus(Guid.NewGuid(), new OrderUpdateRequestModel { OrderId = Guid.NewGuid(), Status = "Processing" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Given_MissingOrder_When_UpdateOrderStatus_Then_ReturnsNotFound()
    {
        var service = new FakeOrderService { UpdateStatusResult = false };
        var orderId = Guid.NewGuid();
        var result = await new OrderController(service, new FakeTrackingService()).UpdateOrderStatus(orderId, new OrderUpdateRequestModel { OrderId = orderId, Status = "Processing" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Given_ValidOrderStatus_When_UpdateOrderStatus_Then_ReturnsOk()
    {
        var orderId = Guid.NewGuid();
        var result = await new OrderController(new FakeOrderService(), new FakeTrackingService()).UpdateOrderStatus(orderId, new OrderUpdateRequestModel { OrderId = orderId, Status = "Processing" });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Given_InvalidClaim_When_CancelOrderAndCreateOrder_Then_ReturnsUnauthorized()
    {
        var controller = WithClaim(new OrderController(new FakeOrderService(), new FakeTrackingService()), ClaimTypes.NameIdentifier, "invalid-guid");

        Assert.IsType<UnauthorizedObjectResult>(await controller.CancelOrder(Guid.NewGuid()));
        Assert.IsType<UnauthorizedObjectResult>(await controller.CreateOrder(new OrderCreateRequest()));
    }

    [Fact]
    public async Task Given_ValidClaim_When_CancelOrderAndCreateOrder_Then_ReturnsOk()
    {
        var controller = WithUser(new OrderController(new FakeOrderService(), new FakeTrackingService()), Guid.NewGuid());

        Assert.IsType<OkObjectResult>(await controller.CancelOrder(Guid.NewGuid()));
        Assert.IsType<OkObjectResult>(await controller.CreateOrder(new OrderCreateRequest()));
    }

    private static T WithUser<T>(T controller, Guid userId) where T : ControllerBase
    {
        return WithClaim(controller, ClaimTypes.NameIdentifier, userId.ToString());
    }

    private static T WithClaim<T>(T controller, string type, string value) where T : ControllerBase
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(type, value) }, "test")) }
        };
        return controller;
    }

    private sealed class FakeCategoryMapper : ICategoryRequestMapper
    {
        public CategoryRequestModel ToApplicationModel(CategoryRequest request) => new() { Id = request.Id, Name = request.Name, Description = request.Description, IsActive = request.IsActive };
    }

    private sealed class FakeCategoryService : ICategoryService
    {
        public Task<IEnumerable<CategoryResponseModel>> GetAllCategoriesAsync() => Task.FromResult<IEnumerable<CategoryResponseModel>>(Array.Empty<CategoryResponseModel>());
        public Task<CategoryResponseModel?> GetCategoryByIdAsync(Guid categoryId) => Task.FromResult<CategoryResponseModel?>(null);
        public Task<CategoryResponseModel> CreateCategoryAsync(CategoryRequestModel request) => Task.FromResult(new CategoryResponseModel { Id = Guid.NewGuid(), Name = request.Name });
        public Task<CategoryResponseModel> UpdateCategoryAsync(CategoryRequestModel request) => Task.FromResult(new CategoryResponseModel { Id = request.Id, Name = request.Name });
        public Task<CategoryResponseModel> DeleteCategoryAsync(Guid categoryId) => Task.FromResult(new CategoryResponseModel { Id = categoryId });
    }

    private sealed class FakeCartService : ICartService
    {
        public Task<CartResponseModel> GetCartAsync(Guid userId) => Task.FromResult(new CartResponseModel());
        public Task<CartResponseModel> AddToCartAsync(Guid userId, AddToCartRequestModel request) => Task.FromResult(new CartResponseModel());
        public Task<CartResponseModel> UpdateCartItemAsync(Guid userId, UpdateCartRequestModel request) => Task.FromResult(new CartResponseModel());
        public Task RemoveCartItemAsync(Guid userId, Guid cartItemId) => Task.CompletedTask;
        public Task ClearCartAsync(Guid userId) => Task.CompletedTask;
    }

    private sealed class FakeProfileService : IProfileService
    {
        public bool ReturnNull { get; set; }
        public UpdateProfileResult UpdateResult { get; set; } = UpdateProfileResult.Success;
        public Task<ProfileResponse?> GetProfileAsync(Guid userId) => Task.FromResult<ProfileResponse?>(ReturnNull ? null : new ProfileResponse { Id = userId, Email = "user@test.com" });
        public Task<UpdateProfileResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request) => Task.FromResult(UpdateResult);
    }

    private sealed class FakeUserQueryService : IUserQueryService
    {
        public Task<IReadOnlyList<UserListItemResponse>> GetAllUsersAsync() => Task.FromResult<IReadOnlyList<UserListItemResponse>>(Array.Empty<UserListItemResponse>());
    }

    private sealed class FakeTrackingService : ITrackingService
    {
        public bool ReturnNull { get; set; }
        public Task<TrackingResponseModel?> GetTrackingDetailsAsync(Guid orderId) => Task.FromResult<TrackingResponseModel?>(ReturnNull ? null : new TrackingResponseModel { OrderId = orderId });
    }

    private sealed class FakeOrderService : IOrderService
    {
        public bool UpdateStatusResult { get; set; } = true;
        public Task<IEnumerable<MyOrderResponseModel>> GetOrdersByUserIdAsync(Guid userId) => Task.FromResult<IEnumerable<MyOrderResponseModel>>(Array.Empty<MyOrderResponseModel>());
        public Task<IEnumerable<MyOrderResponseModel>> GetAllOrdersAsync() => Task.FromResult<IEnumerable<MyOrderResponseModel>>(Array.Empty<MyOrderResponseModel>());
        public Task<bool> UpdateOrderStatusAsync(Guid orderId, string status) => Task.FromResult(UpdateStatusResult);
        public Task<bool> CancelOrderAsync(Guid orderId, Guid userId) => Task.FromResult(true);
        public Task<MyOrderResponseModel> CreateOrderAsync(OrderCreateRequest request) => Task.FromResult(new MyOrderResponseModel());
    }
}
