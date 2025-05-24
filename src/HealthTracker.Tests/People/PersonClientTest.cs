using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.People
{
    [TestClass]
    public class PersonClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IPersonClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Person", Route = "/person" },
                new() { Name = "ExportPerson", Route = "/export/person" },
                new() { Name = "ImportPerson", Route = "/import/person" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new PersonClient(_httpClient, _settings, provider.Object);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [TestMethod]
        public async Task AddTest()
        {
            var person = DataGenerator.RandomPerson(16, 90);
            var json = JsonSerializer.Serialize(person);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(person.FirstNames, person.Surname, person.DateOfBirth, person.Height, person.Gender);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Person").Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(person.Id, added.Id);
            Assert.AreEqual(person.FirstNames, added.FirstNames);
            Assert.AreEqual(person.Surname, added.Surname);
            Assert.AreEqual(person.DateOfBirth, added.DateOfBirth);
            Assert.AreEqual(person.Height, added.Height);
            Assert.AreEqual(person.Gender, added.Gender);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var person = DataGenerator.RandomPerson(16, 90);
            var json = JsonSerializer.Serialize(person);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(person.Id, person.FirstNames, person.Surname, person.DateOfBirth, person.Height, person.Gender);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "Person").Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(person.Id, updated.Id);
            Assert.AreEqual(person.FirstNames, updated.FirstNames);
            Assert.AreEqual(person.Surname, updated.Surname);
            Assert.AreEqual(person.DateOfBirth, updated.DateOfBirth);
            Assert.AreEqual(person.Height, updated.Height);
            Assert.AreEqual(person.Gender, updated.Gender);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes.First(x => x.Name == "Person").Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var person = DataGenerator.RandomPerson(16, 90);
            var json = JsonSerializer.Serialize(new List<dynamic> { person });
            _httpClient.AddResponse(json);

            var people = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "Person").Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(people);
            Assert.AreEqual(1, people.Count);
            Assert.AreEqual(person.Id, people.First().Id);
            Assert.AreEqual(person.FirstNames, people.First().FirstNames);
            Assert.AreEqual(person.Surname, people.First().Surname);
            Assert.AreEqual(person.DateOfBirth, people.First().DateOfBirth);
            Assert.AreEqual(person.Height, people.First().Height);
            Assert.AreEqual(person.Gender, people.First().Gender);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var person = DataGenerator.RandomPerson(16, 90);
            var record = $"""{person.FirstNames}"",""{person.Surname}"",""{person.DateOfBirth:dd/MM/yyyy}"",""{person.Height}"", ""{person.Gender}""";
            
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportPerson").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            _httpClient.AddResponse("");

            _filePath = DataGenerator.TemporaryCsvFilePath();
            await _client.ExportAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportPerson").Route, _httpClient.Requests[0].Uri);
        }
    }
}
