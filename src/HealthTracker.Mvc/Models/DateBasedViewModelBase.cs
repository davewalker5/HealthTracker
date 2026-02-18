using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Mvc.Models
{
    public abstract class DateBasedReportViewModelBase<T> : PaginatedViewModelBase<T> where T : class
    {
        [DisplayName("From")]
        [DataType(DataType.Date)]
        public DateTime? From { get; set; }

        [DisplayName("To")]
        [DataType(DataType.Date)]
        public DateTime? To { get; set; }
    }
}
