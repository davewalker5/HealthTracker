namespace HealthTracker.Mvc.Models
{
    public class AddWeightViewModel : WeightViewModel
    {
        public string Message { get; set; }

        public AddWeightViewModel()
            => Clear();

        public void Clear()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Weight = 0;
        }
    }
}