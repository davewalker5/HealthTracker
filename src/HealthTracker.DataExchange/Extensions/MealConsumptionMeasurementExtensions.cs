using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class MealConsumptionMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable meal consumption measurement from an meal consumption measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <param name="meals"></param>
        /// <returns></returns>
        public static ExportableMealConsumptionMeasurement ToExportable(this MealConsumptionMeasurement measurement, IEnumerable<Person> people, IEnumerable<Meal> meals)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            var meal = meals.First(x => x.Id == measurement.MealId);
            return new ExportableMealConsumptionMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                MealId = measurement.MealId,
                Meal = meal.Name,
                Quantity = measurement.Quantity,
                Calories = measurement.NutritionalValue?.Calories,
                Fat = measurement.NutritionalValue?.Fat,
                SaturatedFat = measurement.NutritionalValue?.SaturatedFat,
                Protein = measurement.NutritionalValue?.Protein,
                Carbohydrates = measurement.NutritionalValue?.Carbohydrates,
                Sugar = measurement.NutritionalValue?.Sugar,
                Fibre = measurement.NutritionalValue?.Fibre
            };
        }

        /// <summary>
        /// Return a collection of exportable meal consumption measurements from a collection of alcohol consumption measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <param name="meals"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableMealConsumptionMeasurement> ToExportable(
            this IEnumerable<MealConsumptionMeasurement> measurements,
            IEnumerable<Person> people,
            IEnumerable<Meal> meals)
        {
            var exportable = new List<ExportableMealConsumptionMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people, meals));
            }

            return exportable;
        }

        /// <summary>
        /// Return a meal consumption measurement from an exportable meal consumption measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="meals"></param>
        /// <returns></returns>
        public static MealConsumptionMeasurement FromExportable(this ExportableMealConsumptionMeasurement exportable, IEnumerable<Meal> meals)
        {
            var meal = meals.First(x => x.Name.Equals(exportable.Meal, StringComparison.OrdinalIgnoreCase));
            return new MealConsumptionMeasurement
            {
                PersonId = exportable.PersonId,
                Date = exportable.Date,
                MealId = meal.Id,
                Quantity = exportable.Quantity,
                NutritionalValue = new NutritionalValue
                {
                    Calories = exportable.Calories,
                    Fat = exportable.Fat,
                    SaturatedFat = exportable.SaturatedFat,
                    Protein = exportable.Protein,
                    Carbohydrates = exportable.Carbohydrates,
                    Sugar = exportable.Sugar,
                    Fibre = exportable.Fibre
                }
            };
        }

        /// <summary>
        /// Return a collection of meal consumption measurements from a collection of exportable meal consumption measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="meals"></param>
        /// <returns></returns>
        public static IEnumerable<MealConsumptionMeasurement> FromExportable(
            this IEnumerable<ExportableMealConsumptionMeasurement> exportable,
            IEnumerable<Meal> meals)
        {
            var measurements = new List<MealConsumptionMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable(meals));
            }

            return measurements;
        }
    }
}
