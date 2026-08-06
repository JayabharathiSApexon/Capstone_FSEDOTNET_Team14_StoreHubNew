using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreHub.API.Services.Interfaces;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserQueryService _userQueryService;

        public UsersController(IUserQueryService userQueryService)
        {
            _userQueryService = userQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userQueryService.GetAllUsersAsync();

            return Ok(users);
        }
    }
}