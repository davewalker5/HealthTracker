using HealthTracker.Entities.Identity;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Mvc.Models
{
    public class AddPersonViewModel : PersonViewModel
    {
        public string Message { get; set; }

        public void Clear()
        {
            Person.Id = 0;
            Person.FirstNames = "";
            Person.Surname = "";
            Person.DateOfBirth = default;
            Person.Height = 0M;
            Person.Gender = Gender.Unspecified;
            Message = "";
        }
    }
}