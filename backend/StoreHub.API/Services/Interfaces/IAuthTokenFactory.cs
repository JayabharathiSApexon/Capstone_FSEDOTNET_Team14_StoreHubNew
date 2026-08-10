using StoreHub.API.Models.Auth;
using StoreHub.Domain.Entities;

namespace StoreHub.API.Services.Interfaces
{
    public interface IAuthTokenFactory
    {
        AuthResponse Create(User user);
    }
}