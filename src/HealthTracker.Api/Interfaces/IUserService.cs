using HealthTracker.Entities.Identity;

namespace HealthTracker.Api.Interfaces
{
    public interface IUserService
    {
        Task<User> AddUserAsync(string userName, string password);
        Task<string> AuthenticateAsync(string userName, string password);
        Task SetUserPasswordAsync(string userName, string password);
    }
}