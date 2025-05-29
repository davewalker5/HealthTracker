using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;

namespace HealthTracker.Tests
{
    [TestClass]
    public class AlcoholMeasureExtensionsTest
    {
        [TestMethod]
        public void NoneTest()
            => Assert.AreEqual("", AlcoholMeasure.None.ToName());

        [TestMethod]
        public void PintTest()
            => Assert.AreEqual("Pint", AlcoholMeasure.Pint.ToName());

        [TestMethod]
        public void LargeGlassTest()
            => Assert.AreEqual("Large Glass", AlcoholMeasure.LargeGlass.ToName());

        [TestMethod]
        public void MediumGlassTest()
            => Assert.AreEqual("Medium Glass", AlcoholMeasure.MediumGlass.ToName());

        [TestMethod]
        public void SmallGlassTest()
            => Assert.AreEqual("Small Glass", AlcoholMeasure.SmallGlass.ToName());

        [TestMethod]
        public void ShotTest()
            => Assert.AreEqual("Shot", AlcoholMeasure.Shot.ToName());
    }
}