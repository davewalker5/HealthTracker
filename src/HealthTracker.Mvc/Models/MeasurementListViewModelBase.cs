namespace HealthTracker.Mvc.Models
{
    public class MeasurementListViewModelBase<T> : ListViewModelBase<T> where T : class
    {
        public FiltersViewModel Filters { get; set; } = new();
    }
}