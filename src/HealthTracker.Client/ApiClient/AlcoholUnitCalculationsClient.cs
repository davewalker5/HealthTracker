using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;

namespace HealthTracker.Client.ApiClient
{
    public class AlcoholUnitCalculationsClient : HealthTrackerClientBase, IAlcoholUnitCalculationsClient
    {
        private const string RouteKey = "Alcohol";

        public AlcoholUnitCalculationsClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
        {
        }

        /// <summary>
        /// Given an ABV % and a volume, calculate the number of units
        /// </summary>
        /// <param name="abv"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task<decimal> CalculateUnitsAsync(decimal abv, decimal volume)
        {
            var route = $"units/{abv}/{volume}";
            var units = await SendCalculationRequestAsync(route);
            return units;
        }

        /// <summary>
        /// Calculate the number of units in a shot
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<decimal> UnitsPerShot(decimal abv)
            => await SendPreConfiguredCalculationRequestAsync("unitspershot", abv);

        /// <summary>
        /// Calculate the number of units in a pint
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<decimal> UnitsPerPint(decimal abv)
            => await SendPreConfiguredCalculationRequestAsync("unitsperpint", abv);

        /// <summary>
        /// Calculate the number of units in a small glass
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<decimal> UnitsPerSmallGlass(decimal abv)
            => await SendPreConfiguredCalculationRequestAsync("unitspersmallglass", abv);

        /// <summary>
        /// Calculate the number of units in a medium glass
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<decimal> UnitsPerMediumGlass(decimal abv)
            => await SendPreConfiguredCalculationRequestAsync("unitspermediumglass", abv);

        /// <summary>
        /// Calculate the number of units in a large glass
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<decimal> UnitsPerLargeGlass(decimal abv)
            => await SendPreConfiguredCalculationRequestAsync("unitsperlargeglass", abv);

        /// <summary>
        /// Send a calculation request for a pre-configured volume
        /// </summary>
        /// <param name="routePrefix"></param>
        /// <param name="abv"></param>
        /// <returns></returns>
        private async Task<decimal> SendPreConfiguredCalculationRequestAsync(string routePrefix, decimal abv)
            => await SendCalculationRequestAsync($"{routePrefix}/{abv}");

        /// <summary>
        /// Send a calculation request to a specified endpoint and return the result
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<decimal> SendCalculationRequestAsync(string endpoint)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{endpoint}";
            var responseContent = await SendDirectAsync(route, null, HttpMethod.Get);
            var units = decimal.Parse(responseContent);
            return units;
        }
    }
}