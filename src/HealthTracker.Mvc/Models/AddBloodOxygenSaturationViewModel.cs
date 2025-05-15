namespace HealthTracker.Mvc.Models
{
    public class AddBloodOxygenSaturationViewModel : BloodOxygenSaturationViewModel
    {
        public string Message { get; set; } = "";

        public AddBloodOxygenSaturationViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Percentage = 0;
        }
    }
}