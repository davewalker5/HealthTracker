using System.ComponentModel;

namespace HealthTracker.Mvc.Models
{
    public class ExportMeasurementsViewModel : ExportViewModel
    {
        [DisplayName("Person")]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [DisplayName("From")]
        public DateTime From { get; set; }

        [DisplayName("To")]
        public DateTime To { get; set; }
    }
}