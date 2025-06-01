using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;

namespace HealthTracker.Tests.Beverages
{
    [TestClass]
    public class BeverageMeasureExtensionsTest
    {
        [TestMethod]
        public void NoneTest()
            => Assert.AreEqual("", TempBeverageMeasure.None.ToName());

        [TestMethod]
        public void PintTest()
            => Assert.AreEqual("Pint", TempBeverageMeasure.Pint.ToName());

        [TestMethod]
        public void LargeGlassTest()
            => Assert.AreEqual("Large Glass", TempBeverageMeasure.LargeGlass.ToName());

        [TestMethod]
        public void MediumGlassTest()
            => Assert.AreEqual("Medium Glass", TempBeverageMeasure.MediumGlass.ToName());

        [TestMethod]
        public void SmallGlassTest()
            => Assert.AreEqual("Small Glass", TempBeverageMeasure.SmallGlass.ToName());

        [TestMethod]
        public void ShotTest()
            => Assert.AreEqual("Shot", TempBeverageMeasure.Shot.ToName());
    }
}