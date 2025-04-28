using HealthTracker.Data;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Logic.Database
{
    public abstract class DatabaseManagerBase
    {
        protected IHealthTrackerFactory Factory { get; private set; }
        protected HealthTrackerDbContext Context { get { return (Factory.Context as HealthTrackerDbContext); } }

        protected DatabaseManagerBase(IHealthTrackerFactory factory)
        {
            Factory = factory;
        }
    }
}
