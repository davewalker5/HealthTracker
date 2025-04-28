using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.People
{
    [TestClass]
    public class PersonImportTest
    {


        private readonly ExportablePerson _person = DataGenerator.RandomPerson(16, 90).ToExportable();
        private IHealthTrackerFactory _factory;
        private IPersonImporter _importer;
        private string _filePath;

        [TestInitialize]
        public void Initialise()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new PersonImporter(_factory, ExportablePerson.CsvRecordPattern);
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
        public void ConvertSingleObjectFromExportable()
        {
            var person = _person.FromExportable();
            Assert.AreEqual(_person.FirstNames, person.FirstNames);
            Assert.AreEqual(_person.Surname, person.Surname);
            Assert.AreEqual(_person.DateOfBirth, person.DateOfBirth);
            Assert.AreEqual(_person.Height, person.Height);
            Assert.AreEqual(_person.Gender, person.Gender.ToString());
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportablePerson> exportable = [_person];
            var people = exportable.FromExportable();
            Assert.AreEqual(_person.FirstNames, people.First().FirstNames);
            Assert.AreEqual(_person.Surname, people.First().Surname);
            Assert.AreEqual(_person.DateOfBirth, people.First().DateOfBirth);
            Assert.AreEqual(_person.Height, people.First().Height);
            Assert.AreEqual(_person.Gender, people.First().Gender.ToString());
        }

        [TestMethod]
        public async Task ImportPeopleTest()
        {
            var record = $@"""{_person.FirstNames}"",""{_person.Surname}"",""{_person.DateOfBirth:dd/MM/yyyy}"",""{_person.Height}"",""{_person.Gender}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var people = await _factory.People.ListAsync(x => true);
            Assert.AreEqual(1, people.Count);
            Assert.AreEqual(_person.FirstNames, people.First().FirstNames);
            Assert.AreEqual(_person.Surname, people.First().Surname);
            Assert.AreEqual(_person.DateOfBirth, people.First().DateOfBirth);
            Assert.AreEqual(_person.Height, people.First().Height);
            Assert.AreEqual(_person.Gender, people.First().Gender.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidFirstNamesTest()
        {
            var record = $@""""",""{_person.Surname}"",""{_person.DateOfBirth:dd/MM/yyyy}"",""{_person.Height}"",""{_person.Gender}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            
            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidSurnameTest()
        {
            var record = $@"""{_person.FirstNames}"","""",""{_person.DateOfBirth:dd/MM/yyyy}"",""{_person.Height}"",""{_person.Gender}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            
            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidDateOfBirthTest()
        {
            var date = DateTime.Now.AddDays(1);
            var record = $@"""{_person.FirstNames}"",""{_person.Surname}"",""{date:dd/MM/yyyy}"",""{_person.Height}"",""{_person.Gender}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            
            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidHeightTest()
        {
            var record = $@"""{_person.FirstNames}"",""{_person.Surname}"",""{_person.DateOfBirth:dd/MM/yyyy}"",""{0}"",""{_person.Gender}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            
            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidGenderTest()
        {
            var record = $@"""{_person.FirstNames}"",""{_person.Surname}"",""{_person.DateOfBirth:dd/MM/yyyy}"",""{_person.Height}"",""Nope""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            
            await _importer.ImportAsync(_filePath);
        }
    }
}