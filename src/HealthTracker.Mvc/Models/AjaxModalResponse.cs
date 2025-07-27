using HealthTracker.Entities;

namespace HealthTracker.Mvc.Models
{
    public class AjaxModalResponse : HealthTrackerEntityBase
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }
    }
}
