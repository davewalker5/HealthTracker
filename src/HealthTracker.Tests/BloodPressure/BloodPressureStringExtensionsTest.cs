using HealthTracker.Entities.Exceptions;
using HealthTracker.Logic.Extensions;
using HealthTracker.Tests.Mocks;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureStringExtensionsTest
    {
        [TestMethod]
        public void CreateBloodPressureMeasurementFromStringTest()
        {
            (int systolic, int diastolic) = HealthTrackerStringExtensions.ToBloodPressureComponents("110/75");
            Assert.AreEqual(110, systolic);
            Assert.AreEqual(75, diastolic);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void NullPressureMeasurementTest()
        {
            _ = HealthTrackerStringExtensions.ToBloodPressureComponents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void TooFewBloodPressureMeasurementComponentsTest()
        {
            _ = HealthTrackerStringExtensions.ToBloodPressureComponents("110 75");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void TooManyBloodPressureMeasurementComponentsTest()
        {
            _ = HealthTrackerStringExtensions.ToBloodPressureComponents("110/75/65");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void InvalidSystolicBloodPressureTest()
        {
            _ = HealthTrackerStringExtensions.ToBloodPressureComponents("Invalid/75");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMeasurementFormatException))]
        public void InvalidDiastolicBloodPressureTest()
        {
            _ = HealthTrackerStringExtensions.ToBloodPressureComponents("110/Invalid");
        }
    }
}
