using HealthTracker.Data;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Extensions;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Exercise
{
    [TestClass]
    public class ExerciseCalculatorTest
    {
        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public void Initialise()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
        }

        [TestMethod]
        public void SummariseEmptyMeasurementsCollectionTest()
        {
            var summaries = _factory.ExerciseCalculator.Summarise([]);
            Assert.IsNull(summaries);
        }

        [TestMethod]
        public void SummariseNullMeasurementsCollectionTest()
        {
            var summaries = _factory.ExerciseCalculator.Summarise(null);
            Assert.IsNull(summaries);
        }

        [TestMethod]
        public void SummariseForSingleActivityTypeTest()
        {
            var personId = DataGenerator.RandomId();
            var activityTypeId = DataGenerator.RandomId();
            var first = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));

            List<ExerciseMeasurement> measurements = [first, second];
            var summaries = _factory.ExerciseCalculator.Summarise(measurements);

            Assert.AreEqual(1, summaries.Count());
            Assert.AreEqual(2, summaries.First().Count);
            Assert.AreEqual(personId, summaries.First().PersonId);
            Assert.AreEqual(measurements.Min(x => x.Date), summaries.First().From);
            Assert.AreEqual(measurements.Max(x => x.Date), summaries.First().To);
            Assert.AreEqual(activityTypeId, summaries.First().ActivityTypeId);
            Assert.AreEqual(first.Duration + second.Duration, summaries.First().TotalDuration);
            Assert.AreEqual((first.Distance ?? 0) + (second.Distance ?? 0), summaries.First().TotalDistance);
            Assert.AreEqual(first.Calories + second.Calories, summaries.First().TotalCalories);
            Assert.AreEqual(measurements.Min(x => x.MinimumHeartRate), summaries.First().MinimumHeartRate);
            Assert.AreEqual(measurements.Max(x => x.MaximumHeartRate), summaries.First().MaximumHeartRate);
        }

        [TestMethod]
        public void ExcludesZeroMinimumHeartRateTest()
        {
            var personId = DataGenerator.RandomId();
            var activityTypeId = DataGenerator.RandomId();
            var first = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));
            second.MinimumHeartRate = 0;

            var summaries = _factory.ExerciseCalculator.Summarise([first, second]);

            Assert.AreEqual(first.MinimumHeartRate, summaries.First().MinimumHeartRate);
        }

        [TestMethod]
        public void ExcludesZeroMaximumHeartRateTest()
        {
            var personId = DataGenerator.RandomId();
            var activityTypeId = DataGenerator.RandomId();
            var first = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));
            second.MaximumHeartRate = 0;

            var summaries = _factory.ExerciseCalculator.Summarise([first, second]);

            Assert.AreEqual(first.MaximumHeartRate, summaries.First().MaximumHeartRate);
        }

        [TestMethod]
        public void SummariseForMixedActivityTypeTest()
        {
            var personId = DataGenerator.RandomId();
            var firstActivityTypeId =  DataGenerator.RandomId();
            var secondActivityTypeId = firstActivityTypeId + DataGenerator.RandomInt(1, 100);
            var first = DataGenerator.RandomExerciseMeasurement(personId, firstActivityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, secondActivityTypeId, DataGenerator.RandomDateInYear(2025));

            List<ExerciseMeasurement> measurements = [first, second];
            var summaries = _factory.ExerciseCalculator.Summarise(measurements);

            Assert.AreEqual(3, summaries.Count());

            var actual = summaries.First(x => x.ActivityTypeId == firstActivityTypeId);
            Assert.AreEqual(first.PersonId, actual.PersonId);
            Assert.AreEqual(first.Date, actual.From);
            Assert.AreEqual(first.Date, actual.To);
            Assert.AreEqual(first.Duration, actual.TotalDuration);
            Assert.AreEqual(first.Distance ?? 0, actual.TotalDistance);
            Assert.AreEqual(first.Calories, actual.TotalCalories);
            Assert.AreEqual(first.MinimumHeartRate, actual.MinimumHeartRate);
            Assert.AreEqual(first.MaximumHeartRate, actual.MaximumHeartRate);

            actual = summaries.First(x => x.ActivityTypeId == secondActivityTypeId);
            Assert.AreEqual(second.PersonId, actual.PersonId);
            Assert.AreEqual(second.Date, actual.From);
            Assert.AreEqual(second.Date, actual.To);
            Assert.AreEqual(second.Duration, actual.TotalDuration);
            Assert.AreEqual(second.Distance ?? 0, actual.TotalDistance);
            Assert.AreEqual(second.Calories, actual.TotalCalories);
            Assert.AreEqual(second.MinimumHeartRate, actual.MinimumHeartRate);
            Assert.AreEqual(second.MaximumHeartRate, actual.MaximumHeartRate);

            var expectedMinHR = first.MinimumHeartRate < second.MinimumHeartRate ? first.MinimumHeartRate : second.MinimumHeartRate;
            var expectedMaxHR = first.MaximumHeartRate > second.MaximumHeartRate ? first.MaximumHeartRate : second.MaximumHeartRate;

            actual = summaries.First(x => x.ActivityTypeId == null);
            Assert.AreEqual(first.PersonId, actual.PersonId);
            Assert.AreEqual(first.Date, actual.From);
            Assert.AreEqual(second.Date, actual.To);
            Assert.AreEqual(first.Duration + second.Duration, actual.TotalDuration);
            Assert.AreEqual((first.Distance ?? 0) + (second.Distance ?? 0), actual.TotalDistance);
            Assert.AreEqual(first.Calories + second.Calories, actual.TotalCalories);
            Assert.AreEqual(expectedMinHR, actual.MinimumHeartRate);
            Assert.AreEqual(expectedMaxHR, actual.MaximumHeartRate);
        }

        [TestMethod]
        public async Task SummariseForPersonAndDateRangeTest()
        {
            var personId = DataGenerator.RandomId();
            var firstActivityTypeId =  DataGenerator.RandomId();
            var secondActivityTypeId = firstActivityTypeId + DataGenerator.RandomInt(1, 100);
            var first = DataGenerator.RandomExerciseMeasurement(personId, firstActivityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, secondActivityTypeId, DataGenerator.RandomDateInYear(2025));

            var context = _factory.Context as HealthTrackerDbContext;
            await context.ExerciseMeasurements.AddAsync(first);
            await context.ExerciseMeasurements.AddAsync(second);
            await context.SaveChangesAsync();

            var summaries = await _factory.ExerciseCalculator.SummariseAsync(first.PersonId, first.Date, second.Date);

            Assert.AreEqual(3, summaries.Count());

            var actual = summaries.First(x => x.ActivityTypeId == firstActivityTypeId);
            Assert.AreEqual(first.PersonId, actual.PersonId);
            Assert.AreEqual(first.Date, actual.From);
            Assert.AreEqual(first.Date, actual.To);
            Assert.AreEqual(first.Duration, actual.TotalDuration);
            Assert.AreEqual(first.Distance ?? 0, actual.TotalDistance);
            Assert.AreEqual(first.Calories, actual.TotalCalories);
            Assert.AreEqual(first.MinimumHeartRate, actual.MinimumHeartRate);
            Assert.AreEqual(first.MaximumHeartRate, actual.MaximumHeartRate);

            actual = summaries.First(x => x.ActivityTypeId == secondActivityTypeId);
            Assert.AreEqual(second.PersonId, actual.PersonId);
            Assert.AreEqual(second.Date, actual.From);
            Assert.AreEqual(second.Date, actual.To);
            Assert.AreEqual(second.Duration, actual.TotalDuration);
            Assert.AreEqual(second.Distance ?? 0, actual.TotalDistance);
            Assert.AreEqual(second.Calories, actual.TotalCalories);
            Assert.AreEqual(second.MinimumHeartRate, actual.MinimumHeartRate);
            Assert.AreEqual(second.MaximumHeartRate, actual.MaximumHeartRate);

            var expectedMinHR = first.MinimumHeartRate < second.MinimumHeartRate ? first.MinimumHeartRate : second.MinimumHeartRate;
            var expectedMaxHR = first.MaximumHeartRate > second.MaximumHeartRate ? first.MaximumHeartRate : second.MaximumHeartRate;

            actual = summaries.First(x => x.ActivityTypeId == null);
            Assert.AreEqual(first.PersonId, actual.PersonId);
            Assert.AreEqual(first.Date, actual.From);
            Assert.AreEqual(second.Date, actual.To);
            Assert.AreEqual(first.Duration + second.Duration, actual.TotalDuration);
            Assert.AreEqual((first.Distance ?? 0) + (second.Distance ?? 0), actual.TotalDistance);
            Assert.AreEqual(first.Calories + second.Calories, actual.TotalCalories);
            Assert.AreEqual(expectedMinHR, actual.MinimumHeartRate);
            Assert.AreEqual(expectedMaxHR, actual.MaximumHeartRate);
        }

        [TestMethod]
        public async Task SummariseForPersonActivityAndDateRangeTest()
        {
            var personId = DataGenerator.RandomId();
            var activityTypeId = DataGenerator.RandomId();
            var first = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2025));

            var context = _factory.Context as HealthTrackerDbContext;
            await context.ExerciseMeasurements.AddAsync(first);
            await context.ExerciseMeasurements.AddAsync(second);
            await context.SaveChangesAsync();

            var summaries = await _factory.ExerciseCalculator.SummariseAsync(first.PersonId, first.Date, second.Date);

            var expectedMinHR = first.MinimumHeartRate < second.MinimumHeartRate ? first.MinimumHeartRate : second.MinimumHeartRate;
            var expectedMaxHR = first.MaximumHeartRate > second.MaximumHeartRate ? first.MaximumHeartRate : second.MaximumHeartRate;

            Assert.AreEqual(1, summaries.Count());
            Assert.AreEqual(2, summaries.First().Count);
            Assert.AreEqual(personId, summaries.First().PersonId);
            Assert.AreEqual(first.Date, summaries.First().From);
            Assert.AreEqual(second.Date, summaries.First().To);
            Assert.AreEqual(activityTypeId, summaries.First().ActivityTypeId);
            Assert.AreEqual(first.Duration + second.Duration, summaries.First().TotalDuration);
            Assert.AreEqual((first.Distance ?? 0) + (second.Distance ?? 0), summaries.First().TotalDistance);
            Assert.AreEqual(first.Calories + second.Calories, summaries.First().TotalCalories);
            Assert.AreEqual(expectedMinHR, summaries.First().MinimumHeartRate);
            Assert.AreEqual(expectedMaxHR, summaries.First().MaximumHeartRate);
        }

        [TestMethod]
        public async Task SummariseForPersonAndNumberOfDaysTest()
        {
            var personId = DataGenerator.RandomId();
            var firstActivityTypeId =  DataGenerator.RandomId();
            var secondActivityTypeId = firstActivityTypeId + DataGenerator.RandomInt(1, 100);
            var first = DataGenerator.RandomExerciseMeasurement(personId, firstActivityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, secondActivityTypeId, HealthTrackerDateExtensions.TodayWithoutTime());

            var context = _factory.Context as HealthTrackerDbContext;
            await context.ExerciseMeasurements.AddAsync(first);
            await context.ExerciseMeasurements.AddAsync(second);
            await context.SaveChangesAsync();

            var days = (int)(second.Date - first.Date).TotalDays + 2;
            var summaries = await _factory.ExerciseCalculator.SummariseAsync(first.PersonId, days);

            Assert.AreEqual(3, summaries.Count());

            var actual = summaries.First(x => x.ActivityTypeId == firstActivityTypeId);
            Assert.AreEqual(first.PersonId, actual.PersonId);
            Assert.AreEqual(first.Date, actual.From);
            Assert.AreEqual(first.Date, actual.To);
            Assert.AreEqual(first.Duration, actual.TotalDuration);
            Assert.AreEqual(first.Distance ?? 0, actual.TotalDistance);
            Assert.AreEqual(first.Calories, actual.TotalCalories);
            Assert.AreEqual(first.MinimumHeartRate, actual.MinimumHeartRate);
            Assert.AreEqual(first.MaximumHeartRate, actual.MaximumHeartRate);

            actual = summaries.First(x => x.ActivityTypeId == secondActivityTypeId);
            Assert.AreEqual(second.PersonId, actual.PersonId);
            Assert.AreEqual(second.Date, actual.From);
            Assert.AreEqual(second.Date, actual.To);
            Assert.AreEqual(second.Duration, actual.TotalDuration);
            Assert.AreEqual(second.Distance ?? 0, actual.TotalDistance);
            Assert.AreEqual(second.Calories, actual.TotalCalories);
            Assert.AreEqual(second.MinimumHeartRate, actual.MinimumHeartRate);
            Assert.AreEqual(second.MaximumHeartRate, actual.MaximumHeartRate);

            var expectedMinHR = first.MinimumHeartRate < second.MinimumHeartRate ? first.MinimumHeartRate : second.MinimumHeartRate;
            var expectedMaxHR = first.MaximumHeartRate > second.MaximumHeartRate ? first.MaximumHeartRate : second.MaximumHeartRate;

            actual = summaries.First(x => x.ActivityTypeId == null);
            Assert.AreEqual(first.PersonId, actual.PersonId);
            Assert.AreEqual(first.Date, actual.From);
            Assert.AreEqual(second.Date, actual.To);
            Assert.AreEqual(first.Duration + second.Duration, actual.TotalDuration);
            Assert.AreEqual((first.Distance ?? 0) + (second.Distance ?? 0), actual.TotalDistance);
            Assert.AreEqual(first.Calories + second.Calories, actual.TotalCalories);
            Assert.AreEqual(expectedMinHR, actual.MinimumHeartRate);
            Assert.AreEqual(expectedMaxHR, actual.MaximumHeartRate);
        }

        [TestMethod]
        public async Task SummariseForPersonActivityAndNumberOfDaysTest()
        {
            var personId = DataGenerator.RandomId();
            var activityTypeId = DataGenerator.RandomId();
            var first = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, DataGenerator.RandomDateInYear(2024));
            var second = DataGenerator.RandomExerciseMeasurement(personId, activityTypeId, HealthTrackerDateExtensions.TodayWithoutTime());

            var context = _factory.Context as HealthTrackerDbContext;
            await context.ExerciseMeasurements.AddAsync(first);
            await context.ExerciseMeasurements.AddAsync(second);
            await context.SaveChangesAsync();

            var days = (int)(second.Date - first.Date).TotalDays + 2;
            var summaries = await _factory.ExerciseCalculator.SummariseAsync(first.PersonId, days);

            var expectedMinHR = first.MinimumHeartRate < second.MinimumHeartRate ? first.MinimumHeartRate : second.MinimumHeartRate;
            var expectedMaxHR = first.MaximumHeartRate > second.MaximumHeartRate ? first.MaximumHeartRate : second.MaximumHeartRate;

            Assert.AreEqual(1, summaries.Count());
            Assert.AreEqual(2, summaries.First().Count);
            Assert.AreEqual(personId, summaries.First().PersonId);
            Assert.AreEqual(first.Date, summaries.First().From);
            Assert.AreEqual(second.Date, summaries.First().To);
            Assert.AreEqual(activityTypeId, summaries.First().ActivityTypeId);
            Assert.AreEqual(first.Duration + second.Duration, summaries.First().TotalDuration);
            Assert.AreEqual((first.Distance ?? 0) + (second.Distance ?? 0), summaries.First().TotalDistance);
            Assert.AreEqual(first.Calories + second.Calories, summaries.First().TotalCalories);
            Assert.AreEqual(expectedMinHR, summaries.First().MinimumHeartRate);
            Assert.AreEqual(expectedMaxHR, summaries.First().MaximumHeartRate);
        }
    }
}