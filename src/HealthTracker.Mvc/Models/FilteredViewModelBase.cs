namespace HealthTracker.Mvc.Models
{
    public class FilteredViewModelBase<T> : ListViewModelBase<T> where T : class
    {
        public FiltersViewModel Filters { get; set; } = new();
    }
}