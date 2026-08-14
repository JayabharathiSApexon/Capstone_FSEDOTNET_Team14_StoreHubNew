using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Profile;
using System.Security.Claims;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var profile = await _profileService.GetProfileAsync(userId.Value);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Profile not found."
                });
            }

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _profileService.UpdateProfileAsync(userId.Value, request);

            return result switch
            {
                UpdateProfileResult.Success => Ok(new
                {
                    message = "Profile updated successfully."
                }),

                UpdateProfileResult.UserNotFound => NotFound(new
                {
                    message = "Profile not found."
                }),

                UpdateProfileResult.EmailAlreadyExists => Conflict(new
                {
                    message = "Email is already registered."
                }),

                _ => StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "An unexpected error occurred."
                    })
            };
        }

        private Guid? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }
}