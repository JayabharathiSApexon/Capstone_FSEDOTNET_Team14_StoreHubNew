using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreHub.Application.Interfaces.Services;
using System.Security.Claims;
using StoreHub.Application.Models.Order;

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
            var result = await _trackingService.GetTrackingDetailsAsync(orderId);

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();

                return Ok(orders);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An error occurred while retrieving orders."
                    });
            }
        }

        [HttpPut("{orderId:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] OrderUpdateRequestModel request)
        {
            try
            {
                if (orderId != request.OrderId)
                {
                    return BadRequest(new
                    {
                        Message = "Order ID mismatch."
                    });
                }

                var result = await _orderService.UpdateOrderStatusAsync(orderId, request.Status);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Order not found."
                    });
                }

                return Ok(new
                {
                    Message = "Order status updated successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{orderId:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new
                    {
                        Message = "User id claim not found or invalid."
                    });
                }

                var result = await _orderService.CancelOrderAsync(orderId, userId);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Order not found."
                    });
                }

                return Ok(new
                {
                    Message = "Order cancelled successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new
                    {
                        Message = "User id claim not found or invalid."
                    });
                }

                request.UserId = userId;

                var result = await _orderService.CreateOrderAsync(request);

                return Ok(new
                {
                    Message = "Order placed successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}
