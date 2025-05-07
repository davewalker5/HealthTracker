using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthTracker.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class FiltersViewModel
    {
        public IList<SelectListItem> People { get; set; } = [];

        [DisplayName("People")]
        [Required(ErrorMessage = "You must select a person")]
        public int PersonId { get; set; }

        [DisplayName("From")]
        [Required(ErrorMessage = "You must provide a 'from' date'")]
        public DateTime From { get; set; }

        [DisplayName("To")]
        [Required(ErrorMessage = "You must provide a 'to' date'")]
        public DateTime To { get; set; }

        public Person SelectedPerson { get; set; }
    }
}