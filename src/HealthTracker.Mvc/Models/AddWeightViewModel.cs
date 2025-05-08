namespace HealthTracker.Mvc.Models
{
    public class AddWeightViewModel : WeightViewModel
    {
        public string Message { get; set; }

        public void Clear()
        {
            Measurement.Id = 0;
            Measurement.Weight = 0;
        }
    }
}