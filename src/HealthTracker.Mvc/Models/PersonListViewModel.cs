using HealthTracker.Entities.Identity;

namespace HealthTracker.Mvc.Models
{
    public class PersonListViewModel : PaginatedViewModelBase<Person>
    {
        public IEnumerable<Person> People => Entities;
    }
}