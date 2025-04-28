using HealthTracker.Logic.Extensions;
using HealthTracker.Tests.Mocks;

namespace HealthTracker.Tests.Extensions
{
    [TestClass]
    public class HealthTrackerDateExtensionsTest
    {
        [TestMethod]
        public void DateWithoutTimeTest()
        {
            var year = DataGenerator.RandomInt(1900, DateTime.Now.Year);
            var date = DataGenerator.RandomDateInYear(year);
            var dateWithoutTime = HealthTrackerDateExtensions.DateWithoutTime(date);
            Assert.AreEqual(year, date.Year);
            Assert.AreEqual(date.Year, dateWithoutTime.Year);
            Assert.AreEqual(date.Month, dateWithoutTime.Month);
            Assert.AreEqual(date.Day, dateWithoutTime.Day);
            Assert.AreEqual(0, dateWithoutTime.Hour);
            Assert.AreEqual(0, dateWithoutTime.Minute);
            Assert.AreEqual(0, dateWithoutTime.Second);
        }

        [TestMethod]
        public void TodayWithoutTimeTest()
        {
            var today = DateTime.Now;
            var date = HealthTrackerDateExtensions.TodayWithoutTime();
            Assert.AreEqual(today.Year, date.Year);
            Assert.AreEqual(today.Month, date.Month);
            Assert.AreEqual(today.Day, date.Day);
            Assert.AreEqual(0, date.Hour);
            Assert.AreEqual(0, date.Minute);
            Assert.AreEqual(0, date.Second);
        }
    }
}