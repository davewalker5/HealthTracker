using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BeverageConsumptionSummaryListViewModel
    {
        public IEnumerable<BeverageConsumptionSummary> Summaries  { get; set; }
    }
}