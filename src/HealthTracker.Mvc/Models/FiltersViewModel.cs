using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Mvc.Models
{
    public class FiltersViewModel : PersonFilterViewModel
    {
        [DisplayName("From")]
        [Required(ErrorMessage = "You must provide a 'from' date'")]
        public DateTime From { get; set; }

        [DisplayName("To")]
        [Required(ErrorMessage = "You must provide a 'to' date'")]
        public DateTime To { get; set; }
    }
}