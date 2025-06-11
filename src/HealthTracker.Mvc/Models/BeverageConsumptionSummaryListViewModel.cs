using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class BeverageConsumptionSummaryListViewModel
    {
        public IEnumerable<BeverageConsumptionSummary> Summaries  { get; set; }
    }
}