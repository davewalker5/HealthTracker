using HealthTracker.Entities.Identity;

namespace HealthTracker.Mvc.Models
{
    public class PersonListViewModel : ListViewModelBase<Person>
    {
        public IEnumerable<Person> People => Entities;
    }
}