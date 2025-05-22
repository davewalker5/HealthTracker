namespace HealthTracker.Mvc.Models
{
    public class HomeViewModel
    {
        public PersonFilterViewModel Filters { get; set; } = new();
        public WeightListViewModel WeightMeasurements { get; set; }
        public ExerciseSummaryListViewModel ExerciseSummaries { get; set; }
        public PersonMedicationListViewModel PersonMedications { get; set; }
    }
}