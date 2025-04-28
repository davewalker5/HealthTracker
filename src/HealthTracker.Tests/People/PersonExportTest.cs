using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Identity;
using HealthTracker.Tests.Mocks;

namespace HealthTracker.Tests.People
{
    [TestClass]
    public class PersonExportTest
    {
        private Person _person = DataGenerator.RandomPerson(16, 90);
        private string _filePath;

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [TestMethod]
        public void ConvertSingleObjectToExportable()
        {
            var exportable = _person.ToExportable();
            Assert.AreEqual(_person.FirstNames, exportable.FirstNames);
            Assert.AreEqual(_person.Surname, exportable.Surname);
            Assert.AreEqual(_person.DateOfBirth, exportable.DateOfBirth);
            Assert.AreEqual(_person.Height, exportable.Height);
            Assert.AreEqual(_person.Gender.ToString(), exportable.Gender);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<Person> people = [_person];
            var exportable = people.ToExportable();
            Assert.AreEqual(_person.FirstNames, exportable.First().FirstNames);
            Assert.AreEqual(_person.Surname, exportable.First().Surname);
            Assert.AreEqual(_person.DateOfBirth, exportable.First().DateOfBirth);
            Assert.AreEqual(_person.Height, exportable.First().Height);
            Assert.AreEqual(_person.Gender.ToString(), exportable.First().Gender);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $"""{_person.FirstNames}"",""{_person.Surname}"",""{_person.DateOfBirth:dd/MM/yyyy}"",""{_person.Height}"",""{_person.Gender}""";
            var exportable = ExportablePerson.FromCsv(record);
            Assert.AreEqual(_person.FirstNames, exportable.FirstNames);
            Assert.AreEqual(_person.Surname, exportable.Surname);
            Assert.AreEqual(_person.DateOfBirth, exportable.DateOfBirth);
            Assert.AreEqual(_person.Height, exportable.Height);
            Assert.AreEqual(_person.Gender.ToString(), exportable.Gender);
        }


        [TestMethod]
        public void ExportPeopleTest()
        {
            List<Person> people = [_person];

            _filePath = DataGenerator.TemporaryCsvFilePath();
            new PersonExporter().ExportAsync(people, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportablePerson.FromCsv(records[1]);
            Assert.AreEqual(_person.FirstNames, exportable.FirstNames);
            Assert.AreEqual(_person.Surname, exportable.Surname);
            Assert.AreEqual(_person.DateOfBirth, exportable.DateOfBirth);
            Assert.AreEqual(_person.Height, exportable.Height);
            Assert.AreEqual(_person.Gender.ToString(), exportable.Gender);
        }
    }
}