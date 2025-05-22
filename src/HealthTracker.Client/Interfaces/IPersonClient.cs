using HealthTracker.Entities.Identity;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Client.Interfaces
{
    public interface IPersonClient
    {
        Task<Person> AddAsync(string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender);
        Task DeleteAsync(int id);
        Task ExportAsync(string fileName);
        Task ImportAsync(string filePath);
        Task<List<Person>> ListAsync(int pageNumber, int pageSize);
        Task<Person> UpdateAsync(int id, string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender);
    }
}