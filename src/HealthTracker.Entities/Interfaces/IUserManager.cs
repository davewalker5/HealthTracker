using HealthTracker.Entities.Identity;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IUserManager
    {
        Task<User> AddAsync(string userName, string password);
        Task<bool> AuthenticateAsync(string userName, string password);
        Task DeleteAsync(string userName);
        Task<User> GetAsync(Expression<Func<User, bool>> predicate);
        Task<List<User>> ListAsync(Expression<Func<User, bool>> predicate);
        Task SetPasswordAsync(string userName, string password);
    }
}
