using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Helpers
{
    public class FoodSourceFilterGenerator : IFoodSourceFilterGenerator
    {
        private readonly IFoodSourceClient _client;
        private readonly IFoodSourceListGenerator _generator;
        private readonly ILogger<FilterGenerator> _logger;

        public FoodSourceFilterGenerator(
            IFoodSourceClient client,
            IFoodSourceListGenerator generator,
            ILogger<FilterGenerator> logger)
        {
            _client = client;
            _generator = generator;
            _logger = logger;
        }

        /// <summary>
        /// Create a food source filter view model with filter properties selected
        /// </summary>
        /// <param name="foodSourceId"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<FoodSourceFilterViewModel> Create(int foodSourceId, ViewFlags flags)
        {
            // Create a new model and populate the list of food sources
            var model = new FoodSourceFilterViewModel()
            {
                FoodSourceId = foodSourceId,
                ShowAddButton = flags.HasFlag(ViewFlags.Add),
                ShowExportButton = flags.HasFlag(ViewFlags.Export)
            };
            await PopulateFoodSourceList(model);

            return model;
        }

        /// <summary>
        /// Generate a list of select list items for food sources
        /// </summary>
        /// <returns></returns>
        public async Task PopulateFoodSourceList(FoodSourceFilterViewModel model)
            => model.Sources = await _generator.Create();
    }
}