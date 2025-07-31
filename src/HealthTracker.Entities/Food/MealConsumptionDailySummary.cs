using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class MealConsumptionDailySummary : NutritionalValue
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public DateTime Date { get; set; }
    }
}
