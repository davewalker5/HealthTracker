namespace HealthTracker.Mvc.Models
{
    public class FilteredByFoodSourceViewModelBase<T> : ListViewModelBase<T> where T : class
    {
        public FoodSourceFilterViewModel Filters { get; set; } = new();
    }
}