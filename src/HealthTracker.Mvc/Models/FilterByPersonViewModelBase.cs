namespace HealthTracker.Mvc.Models
{
    public class FilteredByPersonViewModelBase<T> : ListViewModelBase<T> where T : class
    {
        public PersonFilterViewModel Filters { get; set; } = new();
    }
}