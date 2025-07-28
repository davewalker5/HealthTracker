namespace HealthTracker.Api.Entities
{
    public class PurgePlannedMealsModel
    {
        public int PersonId { get; set; }
        public DateTime? Cutoff { get; set; }
    }
}
