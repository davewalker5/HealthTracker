using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class AuthenticationClient : HealthTrackerClientBase, IAuthenticationClient
    {
        public AuthenticationClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<AuthenticationClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Authenticate with the service and, if successful, return the JWT token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            // Construct the JSON containing the user credentials
            dynamic credentials = new { UserName = username, Password = password };
            var jsonCredentials = JsonSerializer.Serialize(credentials);

            // Send the request. The route is configured in appsettings.json
            var route = Settings.ApiRoutes.First(r => r.Name == "Authenticate").Route;
            var content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(route, content);

            string token = null;
            if (response.IsSuccessStatusCode)
            {
                // Read the token from the response body and set up the default request
                // authentication header
                token = await response.Content.ReadAsStringAsync();
                SetAuthenticationHeader(token);

                // Instruct the provider to store the token
                TokenProvider.SetToken(token);
            }
            else
            {
                var message = $"{(int)response.StatusCode} : {response.ReasonPhrase}";
                throw new AuthenticationException(message);
            }

            return token;
        }

        /// <summary>
        /// Log out by instructing the token provider to clear the token and clearing the authentication header
        /// </summary>
        public void ClearAuthentication()
        {
            TokenProvider.ClearToken();
            SetAuthenticationHeader("");
        }

        /// <summary>
        /// Check the current token is valid
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public async Task<bool> IsTokenValidAsync()
        {
            var route = Settings.ApiRoutes.First(r => r.Name == "Ping").Route;
            var response = await Client.GetAsync(route);
            return response.IsSuccessStatusCode;
        }
    }
}
