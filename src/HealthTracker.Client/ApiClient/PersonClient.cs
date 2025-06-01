using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class PersonClient : HealthTrackerClientBase, IPersonClient
    {
        private const string RouteKey = "Person";
        private const string ExportRouteKey = "ExportPerson";
        private const string ImportRouteKey = "ImportPerson";

        public PersonClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<PersonClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new person to the database
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="height"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public async Task<Person> AddAsync(string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender)
        {
            dynamic template = new
            {
                FirstNames = firstnames,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                Height = height,
                Gender = gender
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var person = Deserialize<Person>(json);

            return person;
        }

        /// <summary>
        /// Update an existing persons details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="height"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public async Task<Person> UpdateAsync(int id, string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender)
        {
            dynamic template = new
            {
                Id = id,
                FirstNames = firstnames,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                Height = height,
                Gender = gender
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var person = Deserialize<Person>(json);

            return person;
        }

        /// <summary>
        /// Delete a person from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Request an import of measurements from the content of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileContentAsync(string content)
        {
            dynamic data = new{ Content = content };
            var json = Serialize(data);
            await SendIndirectAsync(ImportRouteKey, json, HttpMethod.Post);
        }

        /// <summary>
        /// Request an import of  measurements given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));

        /// <summary>
        /// Request an export of people to a named file in the API export folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportAsync(string fileName)
        {
            dynamic data = new { FileName = fileName };
            var json = Serialize(data);
            await SendIndirectAsync(ExportRouteKey, json, HttpMethod.Post);
        }

        /// <summary>
        /// Return a list of people
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Person>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of people
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no people in the database
            List<Person> people = !string.IsNullOrEmpty(json) ? Deserialize<List<Person>>(json) : null;
            return people;
        }
    }
}
