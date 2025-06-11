using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class BeverageListViewModel : ListViewModelBase<Beverage>
    {
        public IEnumerable<Beverage> Beverages => Entities;
    }
}