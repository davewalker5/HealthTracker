namespace HealthTracker.Api.Entities
{
    public class PurgePlannedMealsWorkItem : BackgroundWorkItem
    {
        public int PersonId { get; set; }
        public DateTime? Cutoff { get; set; }
    }
}
