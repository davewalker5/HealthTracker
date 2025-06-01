using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BeverageMeasureListViewModel : ListViewModelBase<BeverageMeasure>
    {
        public IEnumerable<BeverageMeasure> BeverageMeasures => Entities;
    }
}