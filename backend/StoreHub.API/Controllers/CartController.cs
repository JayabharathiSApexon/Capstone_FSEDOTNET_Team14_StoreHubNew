using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Cart;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();

                var cart = await _cartService.GetCartAsync(userId);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartRequestModel request)
        {
            try
            {
                var userId = GetUserId();

                var cart = await _cartService.AddToCartAsync(userId, request);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{cartItemId:guid}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartItemId, UpdateCartRequestModel request)
        {
            try
            {
                if (cartItemId != request.CartItemId)
                {
                    return BadRequest("Cart Item ID mismatch.");
                }

                var userId = GetUserId();

                var cart = await _cartService.UpdateCartItemAsync(userId, request);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{cartItemId:guid}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            try
            {
                var userId = GetUserId();

                await _cartService.RemoveCartItemAsync(userId, cartItemId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();

                await _cartService.ClearCartAsync(userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("User not found.");
            }

            return Guid.Parse(userId);
        }
    }
}