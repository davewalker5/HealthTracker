using HealthTracker.Data;
using HealthTracker.Entities.Identity;

namespace HealthTracker.Tests
{
    [TestClass]
    public class DbContextExtensionsTest
    {
        [TestMethod]
        public void CanGetDbSetForValidEntityTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var dbSet = context.GetDbSet<Person>();
            Assert.IsNotNull(dbSet);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotGetDbSetForInvalidEntityTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            _ = context.GetDbSet<DbContextExtensionsTest>();
        }
    }
}
