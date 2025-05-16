using HealthTracker.Mvc.Interfaces;

namespace HealthTracker.Mvc.Models
{
    public class SelectedFiltersViewModel : IMeasurementFiltersViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}