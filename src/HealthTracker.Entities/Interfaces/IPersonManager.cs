using HealthTracker.Entities.Identity;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IPersonManager
    {
        Task<Person> AddAsync(string firstnames, string surname, DateTime dob, decimal height, Gender gender);
        Task DeleteAsync(int id);
        Task<Person> GetAsync(Expression<Func<Person, bool>> predicate);
        Task<List<Person>> ListAsync(Expression<Func<Person, bool>> predicate);
        Task<Person> UpdateAsync(int id, string firstnames, string surname, DateTime dob, decimal height, Gender gender);
    }
}