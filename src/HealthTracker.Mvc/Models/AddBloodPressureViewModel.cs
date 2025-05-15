namespace HealthTracker.Mvc.Models
{
    public class AddBloodPressureViewModel : BloodPressureViewModel
    {
        public string Message { get; set; }

        public void Clear()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Systolic = 0;
            Measurement.Diastolic = 0;
        }
    }
}