namespace HealthTracker.Mvc.Models
{
    public class AddActivityTypeViewModel : ActivityTypeViewModel
    {
        public string Message { get; set; } = "";

        public AddActivityTypeViewModel()
        {
            ActivityType.Id = 0;
            ActivityType.Description = "";
        }
    }
}