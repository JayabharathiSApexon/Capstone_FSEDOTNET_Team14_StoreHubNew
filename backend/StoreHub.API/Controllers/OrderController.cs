using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ITrackingService _trackingService;

        public OrderController(IOrderService orderService, ITrackingService trackingService)
        {
            _orderService = orderService;
            _trackingService = trackingService;
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetMyOrders(Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId:guid}/tracking")]
        public async Task<IActionResult> GetTracking(Guid orderId)
        {
            var result = await _trackingService.GetTrackingService(orderId);

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Order not found."
                });
            }

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyOrdersFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "User id claim not found or invalid." });
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
    }
}
