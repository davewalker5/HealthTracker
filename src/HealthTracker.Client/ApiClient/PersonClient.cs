using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Client.ApiClient
{
    public class PersonClient : HealthTrackerClientBase, IPersonClient
    {
        private const string RouteKey = "Person";
        private const string ExportRouteKey = "ExportPerson";
        private const string ImportRouteKey = "ImportPerson";

        public PersonClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
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
        public async Task<Person> AddPersonAsync(string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender)
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
        public async Task<Person> UpdatePersonAsync(int id, string firstnames, string surname, DateTime dateOfBirth, decimal height, Gender gender)
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
        public async Task DeletePersonAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Request an import of people from the content of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportPeopleAsync(string filePath)
        {
            dynamic data = new { Content = File.ReadAllText(filePath) };
            var json = Serialize(data);
            await SendIndirectAsync(ImportRouteKey, json, HttpMethod.Post);
        }

        /// <summary>
        /// Request an export of people to a named file in the API export folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportPeopleAsync(string fileName)
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
        public async Task<List<Person>> ListPeopleAsync(int pageNumber, int pageSize)
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
