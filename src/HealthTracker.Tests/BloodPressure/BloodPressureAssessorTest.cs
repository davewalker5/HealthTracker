using HealthTracker.Data;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureAssessorTest
    {
        private readonly List<BloodPressureBand> _bands = [
            new() { Name = "Isolated Systolic Hypertension", MinimumSystolic = 140, MaximumSystolic = 200, MinimumDiastolic = 0, MaximumDiastolic = 89, Order = 1, MatchAll = true },
            new() { Name = "Grade 3 Hypertension (Severe)", MinimumSystolic = 180, MaximumSystolic = 200, MinimumDiastolic = 110, MaximumDiastolic = 200, Order = 2, MatchAll = false },
            new() { Name = "Grade 2 Hypertension (Moderate)", MinimumSystolic = 160, MaximumSystolic = 179, MinimumDiastolic = 100, MaximumDiastolic = 109, Order = 3, MatchAll = false },
            new() { Name = "Grade 1 Hypertension (Mild)", MinimumSystolic = 140, MaximumSystolic = 159, MinimumDiastolic = 90, MaximumDiastolic = 99, Order = 4, MatchAll = false },
            new() { Name = "High Normal", MinimumSystolic = 130, MaximumSystolic = 139, MinimumDiastolic = 85, MaximumDiastolic = 89, Order = 5, MatchAll = false },
            new() { Name = "Normal", MinimumSystolic = 120, MaximumSystolic = 129, MinimumDiastolic = 80, MaximumDiastolic = 84, Order = 6, MatchAll = false },
            new() { Name = "Optimal", MinimumSystolic = 0, MaximumSystolic = 119, MinimumDiastolic = 0, MaximumDiastolic = 79, Order = 7, MatchAll = false },
        ];

        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public async Task Initialise()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            foreach (var band in _bands)
            {
                await context.BloodPressureBands.AddAsync(band);
            }
            await context.SaveChangesAsync();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
        }

        [TestMethod]
        public async Task OptimalReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 0, 119, 0, 79);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Optimal", measurement.Assessment);
        }

        [TestMethod]
        public async Task NormalReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 120, 129, 80, 84);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Normal", measurement.Assessment);
        }

        [TestMethod]
        public async Task HighNormalSystolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 130, 139, 0, 79);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("High Normal", measurement.Assessment);
        }

        [TestMethod]
        public async Task HighNormalDiastolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 120, 129, 85, 89);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("High Normal", measurement.Assessment);
        }

        [TestMethod]
        public async Task GradeOneSystolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 140, 159, 90, 99);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Grade 1 Hypertension (Mild)", measurement.Assessment);
        }

        [TestMethod]
        public async Task GradeOneDiastolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 120, 129, 90, 99);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Grade 1 Hypertension (Mild)", measurement.Assessment);
        }

        [TestMethod]
        public async Task GradeTwoSystolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 160, 179, 90, 99);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Grade 2 Hypertension (Moderate)", measurement.Assessment);
        }

        [TestMethod]
        public async Task GradeTwoDiastolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 120, 129, 100, 109);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Grade 2 Hypertension (Moderate)", measurement.Assessment);
        }

        [TestMethod]
        public async Task GradeThreeSystolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 180, 200, 90, 99);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Grade 3 Hypertension (Severe)", measurement.Assessment);
        }

        [TestMethod]
        public async Task GradeThreeDiastolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 120, 129, 110, 200);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Grade 3 Hypertension (Severe)", measurement.Assessment);
        }

        [TestMethod]
        public async Task IsolatedSystolicReadingTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 140, 200, 0, 79);
            await _factory.BloodPressureAssessor.Assess([measurement]);
            Assert.AreEqual("Isolated Systolic Hypertension", measurement.Assessment);
        }

        [TestMethod]
        public async Task AssessCollectionTest()
        {
            var measurement = DataGenerator.RandomBloodPressureMeasurement(DataGenerator.RandomId(), 2024, 140, 200, 0, 79);
            List<BloodPressureMeasurement> measurements = [measurement];
            await _factory.BloodPressureAssessor.Assess(measurements);
            Assert.AreEqual("Isolated Systolic Hypertension", measurements.First().Assessment);
        }
    }
}