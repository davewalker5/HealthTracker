using HealthTracker.Mvc.Controllers;
using HealthTracker.Client.Interfaces;

namespace HealthTracker.Mvc.Api
{
    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        public const string TokenSessionKey = "HealthTracker.Token";

        private readonly IHttpContextAccessor _accessor;

        public AuthenticationTokenProvider(IHttpContextAccessor accessor)
            => _accessor = accessor;

        /// <summary>
        /// Return the current API token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
            => _accessor.HttpContext.Session.GetString(TokenSessionKey);

        /// <summary>
        /// Set the API token for the current session
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetToken(string token)
            => _accessor.HttpContext.Session.SetString(TokenSessionKey, token);

        /// <summary>
        /// Clear the current API token from the current session
        /// </summary>
        public void ClearToken()
            => _accessor.HttpContext.Session.Clear();
    }
}