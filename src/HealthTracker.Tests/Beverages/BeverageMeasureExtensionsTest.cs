using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;

namespace HealthTracker.Tests.Beverages
{
    [TestClass]
    public class BeverageMeasureExtensionsTest
    {
        [TestMethod]
        public void NoneTest()
            => Assert.AreEqual("", BeverageMeasure.None.ToName());

        [TestMethod]
        public void PintTest()
            => Assert.AreEqual("Pint", BeverageMeasure.Pint.ToName());

        [TestMethod]
        public void LargeGlassTest()
            => Assert.AreEqual("Large Glass", BeverageMeasure.LargeGlass.ToName());

        [TestMethod]
        public void MediumGlassTest()
            => Assert.AreEqual("Medium Glass", BeverageMeasure.MediumGlass.ToName());

        [TestMethod]
        public void SmallGlassTest()
            => Assert.AreEqual("Small Glass", BeverageMeasure.SmallGlass.ToName());

        [TestMethod]
        public void ShotTest()
            => Assert.AreEqual("Shot", BeverageMeasure.Shot.ToName());
    }
}