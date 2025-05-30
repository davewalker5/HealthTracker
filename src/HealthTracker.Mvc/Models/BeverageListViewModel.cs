using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BeverageListViewModel : ListViewModelBase<Beverage>
    {
        public IEnumerable<Beverage> Beverages => Entities;
    }
}