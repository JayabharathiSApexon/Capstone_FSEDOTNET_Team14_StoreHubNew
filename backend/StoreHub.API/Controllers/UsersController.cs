using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreHub.API.Models.User;
using StoreHub.Infrastructure.Data;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .OrderByDescending(user => user.CreatedDate)
                .Select(user => new UserListItemResponse
                {
                    Id = user.Id.ToString(),
                    FullName = user.FullName,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin,
                    Role = user.IsAdmin ? "Admin" : "Guest/User",
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}