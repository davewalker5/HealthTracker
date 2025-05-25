using System.Collections.Generic;
using System.ComponentModel;

namespace HealthTracker.Mvc.Models
{
    public abstract class DateBasedReportViewModelBase<T> : PaginatedViewModelBase<T> where T : class
    {
        [DisplayName("From")]
        public string From { get; set; }

        [DisplayName("To")]
        public string To { get; set; }
    }
}
