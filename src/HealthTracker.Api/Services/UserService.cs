using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthTracker.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IHealthTrackerFactory _factory;
        private readonly HealthTrackerApplicationSettings _settings;

        public UserService(IHealthTrackerFactory factory, IOptions<HealthTrackerApplicationSettings> settings)
        {
            _factory = factory;
            _settings = settings.Value;
        }

        /// <summary>
        /// Authenticate the specified user and, if successful, return the serialized JWT token
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> AuthenticateAsync(string userName, string password)
        {
            string serializedToken = null;

            bool authenticated = await _factory.Users.AuthenticateAsync(userName, password);
            if (authenticated)
            {
                // The user ID is used to construct the claim
                var user = await _factory.Users.GetAsync(x => x.UserName == userName);

                // Construct the information needed to populate the token descriptor
                byte[] key = Encoding.ASCII.GetBytes(_settings.Secret);
                SigningCredentials credentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
                DateTime expiry = DateTime.UtcNow.AddMinutes(_settings.TokenLifespanMinutes);

                // Create the descriptor containing the information used to create the JWT token
                SecurityTokenDescriptor descriptor = new()
                {
                    Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Name, user.UserName)
                    ]),
                    Expires = expiry,
                    SigningCredentials = credentials
                };

                // Use the descriptor to create the JWT token then serialize it to
                // a string
                JwtSecurityTokenHandler handler = new();
                SecurityToken token = handler.CreateToken(descriptor);
                serializedToken = handler.WriteToken(token);
            }

#pragma warning disable CS8603 // Possible null reference return.
            return serializedToken;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsync(string userName, string password)
        {
            return await _factory.Users.AddAsync(userName, password);
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task SetUserPasswordAsync(string userName, string password)
        {
            await _factory.Users.SetPasswordAsync(userName, password);
        }
    }
}
