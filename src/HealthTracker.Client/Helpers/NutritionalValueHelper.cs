using System.Diagnostics.CodeAnalysis;
using HealthTracker.Client.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.Helpers
{
    [ExcludeFromCodeCoverage]
    public abstract class NutritionalValueHelper<T>
    {
        private readonly INutritionalValueClient _nutritionalValueClient;
        protected ILogger<T> Logger { get; private set; }

        public NutritionalValueHelper(INutritionalValueClient nutritionalValueClient, ILogger<T> logger)
        {
            _nutritionalValueClient = nutritionalValueClient;
            Logger = logger;
        }

        /// <summary>
        /// Add a nutritional value
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        protected async Task<int?> AddNutritionalValue(NutritionalValue template)
        {
            int? nutritionalValueId = null;

            if (template?.HasValues == true)
            {
                Logger.LogDebug($"Adding nutritional value record: Calories = {template?.Calories}, " +
                    $"Fat = {template?.Fat}, " +
                    $"Saturated Fat = {template?.SaturatedFat}, " +
                    $"Protein = {template?.Protein}, " +
                    $"Carbohydrates = {template?.Carbohydrates}, " +
                    $"Sugar = {template?.Sugar}, " +
                    $"Fibre = {template?.Fibre}");

                var added = await _nutritionalValueClient.AddAsync(
                    template.Calories,
                    template.Fat,
                    template.SaturatedFat,
                    template.Protein,
                    template.Carbohydrates,
                    template.Sugar,
                    template.Fibre);

                nutritionalValueId = added.Id;
            }

            return nutritionalValueId;
        }

        /// <summary>
        /// Update an existing nutritional value from the supplied template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        protected async Task<int?> UpdateNutritionalValueAsync(int? id, NutritionalValue template)
        {
            // Assess the state of the associated nutritional value:
            //
            // 1. Existing record, no longer has values => Delete it
            // 1. New record, has no values => No further action
            // 2. New Record, has values => Save it, then associate with the food item
            // 4. Existing record, has values => Update it
            if ((id > 0) && (template?.HasValues != true))
            {
                // Existing record but no longer has any values associated with it and can be deleted
                Logger.LogDebug($"Deleting nutritional value with ID {id}");
                await _nutritionalValueClient.DeleteAsync(id ?? 0);
                id = null;
            }
            else
            {
                // Existing record and still has values so needs to be updated
                Logger.LogDebug($"Updating nutritional value with ID {template.Id} : " +
                    $"Calories = {template.Calories}, " +
                    $"Fat = {template.Fat}, " +
                    $"Saturated Fat = {template.SaturatedFat}, " +
                    $"Protein = {template.Protein}, " +
                    $"Carbohydrates = {template.Carbohydrates}, " +
                    $"Sugar = {template.Sugar}, " +
                    $"Fibre = {template.Fibre}");

                _ = await _nutritionalValueClient.UpdateAsync(
                    id ?? 0,
                    template.Calories,
                    template.Fat,
                    template.SaturatedFat,
                    template.Protein,
                    template.Carbohydrates,
                    template.Sugar,
                    template.Fibre);
            }

            return id;
        }

        /// <summary>
        /// Calculate a set of nutritional values given a base set of values and a quantity
        /// </summary>
        /// <param name="values"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        protected NutritionalValue CalculateNutritionalValues(NutritionalValue values, decimal quantity)
        {
            NutritionalValue calculated = null;

            if (values != null)
            {
                calculated = new()
                {
                    Calories = CalculateValue(values.Calories, quantity),
                    Fat = CalculateValue(values.Fat, quantity),
                    SaturatedFat = CalculateValue(values.SaturatedFat, quantity),
                    Protein = CalculateValue(values.Protein, quantity),
                    Carbohydrates = CalculateValue(values.Carbohydrates, quantity),
                    Sugar = CalculateValue(values.Sugar, quantity),
                    Fibre = CalculateValue(values.Fibre, quantity)
                };
            }

            return calculated;
        }

        /// <summary>
        /// Calculate a nullable nutritional value from the base value and a quantity
        /// </summary>
        /// <param name="value"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private decimal? CalculateValue(decimal? value, decimal quantity)
            => value == null ? null : value * quantity;
    }
}