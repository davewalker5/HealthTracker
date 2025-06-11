using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Helpers
{
    public class FoodCategoryFilterGenerator : IFoodCategoryFilterGenerator
    {
        private readonly IFoodCategoryClient _client;
        private readonly IFoodCategoryListGenerator _generator;
        private readonly ILogger<FilterGenerator> _logger;

        public FoodCategoryFilterGenerator(
            IFoodCategoryClient client,
            IFoodCategoryListGenerator generator,
            ILogger<FilterGenerator> logger)
        {
            _client = client;
            _generator = generator;
            _logger = logger;
        }

        /// <summary>
        /// Create a food category filter view model with filter properties selected
        /// </summary>
        /// <param name="foodCategoryId"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<FoodCategoryFilterViewModel> Create(int foodCategoryId, ViewFlags flags)
        {
            // Create a new model and populate the list of people
            var model = new FoodCategoryFilterViewModel()
            {
                FoodCategoryId = foodCategoryId,
                ShowAddButton = flags.HasFlag(ViewFlags.Add),
                ShowExportButton = flags.HasFlag(ViewFlags.Export)
            };
            await PopulateFoodCategoryList(model);

            return model;
        }

        /// <summary>
        /// Generate a list of select list items for food categories
        /// </summary>
        /// <returns></returns>
        public async Task PopulateFoodCategoryList(FoodCategoryFilterViewModel model)
            => model.Categories = await _generator.Create();
    }
}