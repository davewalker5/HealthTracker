using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;

namespace HealthTracker.Tests
{
    [TestClass]
    public class UnitExtensionsTest
    {
        [TestMethod]
        public void BloodPressureMmHgTest()
            => Assert.AreEqual("mmHg", BloodPressureUnit.MillimetresOfMercury.ToDescription());

        [TestMethod]
        public void CholesterolMmolLTest()
            => Assert.AreEqual("mmol/L", CholesterolUnit.MillimolesPerLitre.ToDescription());

        [TestMethod]
        public void DistanceMetresTest()
            => Assert.AreEqual("m", DistanceUnit.Metres.ToDescription());

        [TestMethod]
        public void DistanceKilometresTest()
            => Assert.AreEqual("km", DistanceUnit.Kilometres.ToDescription());

        [TestMethod]
        public void DistanceMilesTest()
            => Assert.AreEqual("miles", DistanceUnit.Miles.ToDescription());

        [TestMethod]
        public void HeartRateBPMTest()
            => Assert.AreEqual("bpm", HeartRateUnit.BeatsPerMinute.ToDescription());

        [TestMethod]
        public void HeightCentimetresTest()
            => Assert.AreEqual("cm", HeightUnit.Centimetres.ToDescription());

        [TestMethod]
        public void HeightMetresTest()
            => Assert.AreEqual("m", HeightUnit.Metres.ToDescription());

        [TestMethod]
        public void HeightFeetTest()
            => Assert.AreEqual("feet", HeightUnit.Feet.ToDescription());

        [TestMethod]
        public void WeightKilogramsTest()
            => Assert.AreEqual("kg", WeightUnit.Kilograms.ToDescription());

        [TestMethod]
        public void WeightPoundsTest()
            => Assert.AreEqual("lbs", WeightUnit.Pounds.ToDescription());
    }
}