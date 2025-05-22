using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthTracker.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class PersonFilterViewModel
    {
        public IList<SelectListItem> People { get; set; } = [];
        public bool ShowAddButton { get; set; }

        [DisplayName("Person")]
        [Required(ErrorMessage = "You must select a person")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a person")]
        public int PersonId { get; set; }

        public Person SelectedPerson { get; set; }
    }
}