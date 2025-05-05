using HealthTracker.Configuration.Logic;
using HealthTracker.Entities.Logging;

namespace HealthTracker.Tests
{
    [TestClass]
    public class HealthTrackerConfigReaderTest
    {
        [TestMethod]
        public void ReadAppSettingsTest()
        {
            var settings = new HealthTrackerConfigReader().Read("appsettings.json");

            Assert.AreEqual("Some Secret", settings.Secret);
            Assert.AreEqual(1440, settings.TokenLifespanMinutes);
            Assert.AreEqual(Severity.Info, settings.MinimumLogLevel);
            Assert.AreEqual("HealthTracker.log", settings.LogFile);
            Assert.AreEqual("health-tracker-token.dat", settings.TokenFile);
            Assert.AreEqual("https://localhost:12345", settings.ApiUrl);
            Assert.AreEqual("yyyy-MM-dd H:mm:ss", settings.ApiDateFormat);
            Assert.AreEqual("/an/export/folder", settings.ExportPath);
            Assert.AreEqual(14, settings.DefaultTimePeriodDays);
            Assert.AreEqual("Date", settings.OmronDateColumnTitle);
            Assert.AreEqual("Systolic (mmHg)", settings.OmronSystolicColumnTitle);
            Assert.AreEqual("Diastolic (mmHg)", settings.OmronDiastolicColumnTitle);
            Assert.AreEqual(10, settings.MedicationOrderLeadTimeDays);
            Assert.AreEqual(14, settings.MedicationDaysRemainingWarningDays);
            Assert.AreEqual(7, settings.MedicationDaysRemainingCriticalDays);
            Assert.AreEqual(25, settings.ShotSize);
            Assert.AreEqual(125, settings.SmallGlassSize);
            Assert.AreEqual(175, settings.MediumGlassSize);
            Assert.AreEqual(250, settings.LargeGlassSize);
            Assert.AreEqual(10, settings.ResultsPageSize);

            Assert.AreEqual(1, settings.ApiRoutes.Count);
            Assert.AreEqual("A Route", settings.ApiRoutes.First().Name);
            Assert.AreEqual("/some/route", settings.ApiRoutes.First().Route);
        }
    }
}
