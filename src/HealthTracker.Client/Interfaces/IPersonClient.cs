using HealthTracker.Entities.Identity;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Client.Interfaces
{
    public interface IPersonClient
    {
        Task<Person> AddPersonAsync(string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender);
        Task DeletePersonAsync(int id);
        Task ExportPeopleAsync(string fileName);
        Task ImportPeopleAsync(string filePath);
        Task<List<Person>> ListPeopleAsync();
        Task<Person> UpdatePersonAsync(int id, string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender);
    }
}