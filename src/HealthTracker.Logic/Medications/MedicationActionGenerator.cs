using System.Resources;
using System.Reflection;
using HealthTracker.Logic.Extensions;
using HealthTracker.Entities.Medications;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Configuration.Interfaces;

namespace HealthTracker.Logic.Medications
{
    public class MedicationActionGenerator : IMedicationActionGenerator
    {
        private readonly ResourceManager _resources = new("HealthTracker.Logic.Properties.Resources", Assembly.GetExecutingAssembly());
        private readonly IHealthTrackerApplicationSettings _settings;

        internal MedicationActionGenerator(IHealthTrackerApplicationSettings settings)
            => _settings = settings;
        
        /// <summary>
        /// Associate a list of actions with a person/medication association
        /// </summary>
        /// <param name="association"></param>
        /// <returns></returns>
        public void DetermineActions(PersonMedication association)
        {
            association.Actions = [];

            // Check the association is active, first
            if (association.Active)
            {
                if (association.LastTaken < HealthTrackerDateExtensions.TodayWithoutTime())
                {
                    string action = _resources.GetString("TakeDoseAction");
                    association.Actions.Add(action);
                }

                if (association.DaysRemaining() <= _settings.MedicationOrderLeadTimeDays)
                {
                    string action = _resources.GetString("OrderMoreAction");
                    association.Actions.Add(action);
                }
            }
        }

        /// <summary>
        /// Associate a list of actions with each person/medication association in a collection
        /// </summary>
        /// <param name="associations"></param>
        public void DetermineActions(IEnumerable<PersonMedication> associations)
        {
            foreach (var association in associations)
            {
                DetermineActions(association);
            }
        }
    }
}





