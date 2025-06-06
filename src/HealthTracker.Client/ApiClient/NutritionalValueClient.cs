using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class NutritionalValueClient : HealthTrackerClientBase, INutritionalValueClient
    {
        private const string RouteKey = "NutritionalValue";

        public NutritionalValueClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<NutritionalValueClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new nutritional value to the database
        /// </summary>
        /// <param
        /// <param name="calories"></param>
        /// <param name="fat"></param>
        /// <param name="saturatedFat"></param>
        /// <param name="protein"></param>
        /// <param name="carbohydrates"></param>
        /// <param name="sugar"></param>
        /// <param name="fibre"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> AddAsync(
            decimal? calories,
            decimal? fat,
            decimal? saturatedFat,
            decimal? protein,
            decimal? carbohydrates,
            decimal? sugar,
            decimal? fibre)
        {
            dynamic template = new
            {
                Calories = calories,
                Fat = fat,
                SaturatedFat = saturatedFat,
                Protein = protein,
                Carbohydrates = carbohydrates,
                Sugar = sugar,
                Fibre = fibre
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var nutrition = Deserialize<NutritionalValue>(json);

            return nutrition;
        }

        /// <summary>
        /// Update an existing nutritional value
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fat"></param>
        /// <param name="saturatedFat"></param>
        /// <param name="protein"></param>
        /// <param name="carbohydrates"></param>
        /// <param name="sugar"></param>
        /// <param name="fibre"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> UpdateAsync(
            int id,
            decimal? calories,
            decimal? fat,
            decimal? saturatedFat,
            decimal? protein,
            decimal? carbohydrates,
            decimal? sugar,
            decimal? fibre)
        {
            dynamic template = new
            {
                Id = id,
                Calories = calories,
                Fat = fat,
                SaturatedFat = saturatedFat,
                Protein = protein,
                Carbohydrates = carbohydrates,
                Sugar = sugar,
                Fibre = fibre
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var nutrition = Deserialize<NutritionalValue>(json);

            return nutrition;
        }

        /// <summary>
        /// Return a nutritional value set from the database given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> GetAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var nutrition = Deserialize<NutritionalValue>(json);

            return nutrition;
        }

        /// <summary>
        /// Delete a nutritional value set from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }
    }
}
