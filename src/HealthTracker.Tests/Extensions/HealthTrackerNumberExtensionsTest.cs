using HealthTracker.Logic.Extensions;
using HealthTracker.Tests.Mocks;

namespace HealthTracker.Tests.Extensions
{
    [TestClass]
    public class HealthTrackerNumberExtensionsTest
    {
        [TestMethod]
        public void DurationToFormattedStringTest()
        {
            var hours = DataGenerator.RandomInt(0, 99);
            var minutes = DataGenerator.RandomInt(0, 56);
            var seconds = DataGenerator.RandomInt(0, 56);
            var duration = 3600 * hours + 60 * minutes + seconds;
            var formatted = HealthTrackerNumberExtensions.ToFormattedDuration(duration);
            Assert.AreEqual($"{hours:00}:{minutes:00}:{seconds:00}", formatted);
        }
    }
}