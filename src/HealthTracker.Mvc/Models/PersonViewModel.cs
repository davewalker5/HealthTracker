using HealthTracker.Entities.Identity;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class PersonViewModel
    {
        public Person Person { get; set; } = new();
        public IList<SelectListItem> Genders { get; set; }= [];
        public string Action { get; set; }

        public PersonViewModel()
        {
            // Build the gender selection list from the members of the Gender enumeration
            foreach (var gender in Enum.GetValues<Gender>())
            {
                Genders.Add(new SelectListItem() { Text = gender.ToString(), Value = gender.ToString() });
            }

            // Reset the person properties
            Person.Id = 0;
            Person.FirstNames = "";
            Person.Surname = "";
            Person.DateOfBirth = DateTime.Today;
            Person.Height = 0M;
            Person.Gender = Gender.Unspecified;
        }
    }
}