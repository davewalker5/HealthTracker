using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class HomeViewModel
    {
        public PersonFilterViewModel Filters { get; set; } = new();

        public WeightListViewModel WeightMeasurements { get; set; }
        public bool HasWeightMeasurements { get { return WeightMeasurements?.Measurements?.Any() ?? false; } }

        public IEnumerable<MealConsumptionDailySummary> MealConsumption { get; set; }
        public bool HasMealConsumption { get { return MealConsumption?.Any() ?? false; } }

        public BeverageConsumptionListViewModel HydratingBeverageConsumption { get; set; }
        public bool HasHydratingBeverageConsumption { get { return HydratingBeverageConsumption?.Measurements?.Any() ?? false; } }

        public BeverageConsumptionSummaryListViewModel TotalAlcoholConsumption { get; set; }
        public bool HasTotalAlcoholConsumption { get { return (TotalAlcoholConsumption != null) && (TotalAlcoholConsumption.Summaries != null) && (TotalAlcoholConsumption.Summaries.Count() > 0); } }

        public ExerciseSummaryListViewModel ExerciseSummaries { get; set; }
        public bool HasExerciseSummaries { get { return ExerciseSummaries?.Summaries?.Any() ?? false; } }

        public PersonMedicationListViewModel PersonMedications { get; set; }
        public bool HasPersonMedications { get { return PersonMedications?.Associations?.Any() ?? false; } }
    }
}