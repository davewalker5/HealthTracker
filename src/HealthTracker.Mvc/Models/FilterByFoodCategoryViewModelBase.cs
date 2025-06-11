namespace HealthTracker.Mvc.Models
{
    public class FilteredByFoodCategoryViewModelBase<T> : ListViewModelBase<T> where T : class
    {
        public FoodCategoryFilterViewModel Filters { get; set; } = new();
    }
}