using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class ExerciseSummaryListViewModel
    {
        public IEnumerable<ExerciseSummary> Summaries  { get; set; }
    }
}