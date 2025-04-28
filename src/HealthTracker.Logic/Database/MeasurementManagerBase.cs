using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Logic.Database
{
    public abstract class MeasurementManagerBase : DatabaseManagerBase
    {
        protected MeasurementManagerBase(IHealthTrackerFactory factory) : base(factory) {}

        /// <summary>
        /// Check a person with a specified ID exists and raise an exception if not
        /// </summary>
        /// <param name="personId"></param>
        protected void CheckPersonExists(int personId)
        {
            var person = Context.People.FirstOrDefault(x => x.Id == personId);
            if (person == null)
            {
                var message = $"Person with Id {personId} does not exist";
                throw new PersonNotFoundException(message);
            }
        }
    }
}
