using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class FoodSourceListViewModel : ListViewModelBase<FoodSource>
    {
        public IEnumerable<FoodSource> FoodSources => Entities;
    }
}