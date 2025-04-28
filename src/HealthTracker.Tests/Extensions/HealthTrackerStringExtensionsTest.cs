using HealthTracker.Entities.Exceptions;
using HealthTracker.Logic.Extensions;
using HealthTracker.Tests.Mocks;

namespace HealthTracker.Tests.Extensions
{
    [TestClass]
    public class HealthTrackerStringExtensionsTest
    {
        [TestMethod]
        public void SringToBloodPressureTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            (int systolic, int diastolic) = HealthTrackerStringExtensions.ToBloodPressureComponents($"{measurement.Systolic}/{measurement.Diastolic}");
            Assert.AreEqual(measurement.Systolic, systolic);
            Assert.AreEqual(measurement.Diastolic, diastolic);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void TooFewSegmentsBloodPressureExceptionTest()
            => _ = HealthTrackerStringExtensions.ToBloodPressureComponents("123");

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void TooManySegmentsBloodPressureExceptionTest()
            => _ = HealthTrackerStringExtensions.ToBloodPressureComponents("123/74/56");

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void EmptyBloodPressureExceptionTest()
            => _ = HealthTrackerStringExtensions.ToBloodPressureComponents("");

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void NullBloodPressureExceptionTest()
            => _ = HealthTrackerStringExtensions.ToBloodPressureComponents(null);

        [TestMethod]
        public void FormatedDurationToSecondsTest()
        {
            var seconds = HealthTrackerStringExtensions.ToDuration("01:12:34");
            Assert.AreEqual(4354, seconds);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void TooFewSegmentsDurationExceptionTest()
            => _ = HealthTrackerStringExtensions.ToDuration("01:12");

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void TooManySegmentsDurationExceptionTest()
            => _ = HealthTrackerStringExtensions.ToDuration("01:12:34:23");

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void EmptyDurationExceptionTest()
            => _ = HealthTrackerStringExtensions.ToDuration("");

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void NullDurationExceptionTest()
            => _ = HealthTrackerStringExtensions.ToDuration(null);
    }
}