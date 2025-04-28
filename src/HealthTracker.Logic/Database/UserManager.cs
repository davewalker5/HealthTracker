using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class UserManager : DatabaseManagerBase, IUserManager
    {
        private readonly Lazy<PasswordHasher<string>> _hasher;

        internal UserManager(IHealthTrackerFactory factory) : base(factory)
        {
            _hasher = new Lazy<PasswordHasher<string>>(() => new PasswordHasher<string>());
        }

        /// <summary>
        /// Return the first user matching the specified criteria
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetAsync(Expression<Func<User, bool>> predicate)
        {
            List<User> users = await ListAsync(predicate);
            return users.FirstOrDefault();
        }

        /// <summary>
        /// Get all users matching the specified criteria
        /// </summary>
        public Task<List<User>> ListAsync(Expression<Func<User, bool>> predicate)
            => Context.Users.Where(predicate).ToListAsync();

        /// <summary>
        /// Add a user, if they don't already exist
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AddAsync(string userName, string password)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding a new user '{userName}'");

            var user = await GetAsync(u => u.UserName == userName);

            if (user == null)
            {
                user = new User
                {
                    UserName = userName,
                    Password = _hasher.Value.HashPassword(userName, password)
                };

                await Context.Users.AddAsync(user);
                await Context.SaveChangesAsync();
            }

            return user;
        }

        /// <summary>
        /// Authenticate the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            var result = PasswordVerificationResult.Failed;

            // Get the user record and make sure it exists
            var user = await GetAsync(x => x.UserName == userName);
            if (user != null)
            {
                // Attempt to verify the password
                result = _hasher.Value.VerifyHashedPassword(userName, user.Password, password);
                if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    // Verified, but need to rehash the password
                    user.Password = _hasher.Value.HashPassword(userName, password);
                    await Context.SaveChangesAsync();
                }
            }

            Factory.Logger.LogMessage(Severity.Info, $"Authentication result for user '{userName}': {result}");
            return result != PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task SetPasswordAsync(string userName, string password)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Setting the password for user '{userName}'");

            var user = await GetAsync(x => x.UserName == userName);
            if (user != null)
            {
                user.Password = _hasher.Value.HashPassword(userName, password);
                await Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Delete the specified user
        /// </summary>
        /// <param name="userName"></param>
        public async Task DeleteAsync(string userName)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting user '{userName}'");

            var user = await GetAsync(x => x.UserName == userName);
            if (user != null)
            {
                Context.Users.Remove(user);
                await Context.SaveChangesAsync();
            }
        }
    }
}
