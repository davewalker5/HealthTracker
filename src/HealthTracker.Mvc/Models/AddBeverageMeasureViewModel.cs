namespace HealthTracker.Mvc.Models
{
    public class AddBeverageMeasureViewModel : BeverageMeasureViewModel
    {
        public string Message { get; set; } = "";

        public AddBeverageMeasureViewModel()
        {
            Measure.Id = 0;
            Measure.Name = "";
            Measure.Volume = 0;
        }
    }
}