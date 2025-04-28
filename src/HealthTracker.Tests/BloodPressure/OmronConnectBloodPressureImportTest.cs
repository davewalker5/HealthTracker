using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class OmronConnectBloodPressureImportTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private const string FileName = "omron.xlsx";
        private const string DateColumnTitle = "Date";
        private const string SystolicColumnTitle = "Systolic (mmHg)";
        private const string DiastolicColumnTitle = "Diastolic (mmHg)";
        private DateTime MeasurementDate = new(2024, 11, 22, 0, 0, 0);
        private const int Systolic = 111;
        private const int Diastolic = 69;

        private IHealthTrackerFactory _factory;
        private IOmronConnectBloodPressureImporter _importer;
        private byte[] _content;

        [TestInitialize]
        public async Task Initialise()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            var bloodPressureImporter = new BloodPressureMeasurementImporter(_factory, ExportableBloodPressureMeasurement.CsvRecordPattern);
            _importer = new OmronConnectBloodPressureImporter(_factory, bloodPressureImporter, DateColumnTitle, SystolicColumnTitle, DiastolicColumnTitle);

            _content = File.ReadAllBytes(FileName);
        }

        [TestMethod]
        public async Task ByteArrayImportTest()
        {
            await _importer.ImportAsync(_content, _person.Id);
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(x => true);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(MeasurementDate, measurements.First().Date);
            Assert.AreEqual(Systolic, measurements.First().Systolic);
            Assert.AreEqual(Diastolic, measurements.First().Diastolic);
        }

        [TestMethod]
        public async Task Base64StringImportTest()
        {
            var base64 = Convert.ToBase64String(_content);
            await _importer.ImportAsync(base64, _person.Id);
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(x => true);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(MeasurementDate, measurements.First().Date);
            Assert.AreEqual(Systolic, measurements.First().Systolic);
            Assert.AreEqual(Diastolic, measurements.First().Diastolic);
        }
    }
}