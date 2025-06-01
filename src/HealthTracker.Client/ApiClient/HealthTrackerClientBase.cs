using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public abstract class HealthTrackerClientBase
    {
        protected const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly JsonSerializerOptions _serializerOptions = new ()
        {
            PropertyNameCaseInsensitive = true
        };

        protected IHealthTrackerHttpClient Client { get; private set; }
        protected IHealthTrackerApplicationSettings Settings { get; private set; }
        protected IAuthenticationTokenProvider TokenProvider { get; private set; }
        protected ILogger Logger { get; private set; }

        public HealthTrackerClientBase(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger logger)
        {
            Settings = settings;
            TokenProvider = tokenProvider;
            Logger = logger;
            Client = client;
            Client.BaseAddress = new Uri(Settings.ApiUrl);

            string token = tokenProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                SetAuthenticationHeader(token);
            }
        }

        /// <summary>
        /// Add the authorization header to the default request headers
        /// </summary>
        /// <param name="token"></param>
        protected void SetAuthenticationHeader(string token)
            => Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        /// <summary>
        /// Given a route name, some data (null in the case of GET) and an HTTP method,
        /// look up the route from the application settings then send the request to
        /// the service and return the JSON response
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected async Task<string> SendIndirectAsync(string routeName, string data, HttpMethod method)
        {
            string route = Settings.ApiRoutes.First(r => r.Name == routeName).Route;
            string json = await SendDirectAsync(route, data, method);
            return json;
        }

        /// <summary>
        /// Given a route, some data (null in the case of GET) and an HTTP method,
        /// send the request to the service and return the JSON response
        /// </summary>
        /// <param name="route"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected async Task<string> SendDirectAsync(string route, string data, HttpMethod method)
        {
            string json = null;

            var body = !string.IsNullOrEmpty(data) ? data : "No Content";
            Logger.LogDebug($"Sending {method} request to endpoint {route}");
            Logger.LogDebug($"Request body = {body}");

            HttpResponseMessage response = null;
            if (method == HttpMethod.Get)
            {
                response = await Client.GetAsync(route);
            }
            else if (method == HttpMethod.Post)
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await Client.PostAsync(route, content);
            }
            else if (method == HttpMethod.Put)
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await Client.PutAsync(route, content);
            }
            else if (method == HttpMethod.Delete)
            {
                response = await Client.DeleteAsync(route);
            }

            Logger.LogDebug($"HTTP Status Code = {response?.StatusCode}");

            if ((response != null) && response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
                var content = json ?? "No Content";
                Logger.LogDebug($"Response content = '{content}'");
            }

            return json;
        }

        /// <summary>
        /// Serialize an object to JSON
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        protected static string Serialize(object o)
            => JsonSerializer.Serialize(o);

        /// <summary>
        /// Deserialize a JSON string to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        protected T Deserialize<T>(string json) where T : class
            => JsonSerializer.Deserialize<T>(json, _serializerOptions);

        /// <summary>
        /// Calculate a from and to date range from two dates, either or both of which may be null
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected (DateTime fromDate, DateTime toDate) CalculateDateRange(DateTime? from, DateTime? to)
        {
            DateTime fromDate;
            DateTime toDate;

            if ((from == null) && (to == null))
            {
                // End of then range is now and the start is the default time period ago
                toDate = DateTime.Now;
                fromDate = toDate.AddDays(-Settings.DefaultTimePeriodDays);
            }
            else if ((from == null) && (to != null))
            {
                // End of then range is the specified date and the start is the default time period earlier
                toDate = to.Value;
                fromDate = toDate.AddDays(-Settings.DefaultTimePeriodDays);
            }
            else if ((from != null) && (to == null))
            {
                // The from date is the specified date and the end date is now
                fromDate = from.Value;
                toDate = DateTime.Now;
            }
            else
            {
                fromDate = from.Value;
                toDate = to.Value;
            }

            return (fromDate, toDate);
        }

        /// <summary>
        /// Calculate a from and to date range from two dates, either or both of which may be null, and return
        /// the URL encoded result
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected (string encodedFromDate, string encodedToDate) CalculateEncodedDateRange(DateTime? from, DateTime? to)
        {
            // Determine the date range
            (DateTime fromDate, DateTime toDate) = CalculateDateRange(from, to);

            // Encode the dates
            var encodedFromDate = HttpUtility.UrlEncode(fromDate.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(toDate.ToString(DateTimeFormat));

            return (encodedFromDate, encodedToDate);
        }
    }
}
