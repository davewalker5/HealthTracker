using HealthTracker.Entities.Identity;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class PersonViewModelBase
    {
        public Person Person { get; set; } = new();
        public IList<SelectListItem> Genders = [];

        public PersonViewModelBase()
        {
            // Build the gender selection list from the members of the Gender enumeration
            foreach (var gender in Enum.GetValues<Gender>())
            {
                Genders.Add(new SelectListItem() { Text = gender.ToString(), Value = gender.ToString() });
            }
        }
    }
}