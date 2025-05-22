using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Extensions;

namespace HealthTracker.Logic.Calculations
{
    public class ExerciseCalculator : IExerciseCalculator
    {
        private readonly IHealthTrackerFactory _factory;

        internal ExerciseCalculator(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Summarise a set of measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExerciseSummary>> Summarise(IEnumerable<ExerciseMeasurement> measurements)
        {
            List<ExerciseSummary> summaries = null;

            // Check there are some measurements
            if (measurements?.Count() > 0)
            {
                // This adds summaries for each person/activity combination
                summaries = [.. measurements.GroupBy(x => new { x.PersonId, x.ActivityTypeId }).Select(SummariseMeasurementGroup)];

                // Iterate over the group of measurements for each person
                foreach (var group in measurements.GroupBy(x => x.PersonId).Select(x => x.ToList()))
                {
                    // See how many activity types there are in the group. If there's only one, then the existing
                    // summary entry for the associated person will be the total and there's no need to add another
                    if (group.Select(x => x.ActivityTypeId).Distinct().Count() > 1)
                    {
                        summaries.Add(SummariseMeasurementGroup(group));
                    }
                }

                // Populate the name of the person and the activity name, if possible, and set the formatted duration
                var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);
                var activityTypes = await _factory.ActivityTypes.ListAsync(x => true, 1, int.MaxValue);
                foreach (var summary in summaries)
                {
                    summary.FormattedDuration = summary.TotalDuration.ToFormattedDuration();

                    if (summary.PersonId != null)
                    {
                        summary.PersonName = people.FirstOrDefault(x => x.Id == summary.PersonId)?.Name;
                    }

                    if (summary.ActivityTypeId != null)
                    {
                        summary.ActivityDescription = activityTypes.FirstOrDefault(x => x.Id == summary.ActivityTypeId)?.Description;
                    }
                }
            }

            return summaries;
        }

        /// <summary>
        /// Summarise exercise records in a date range for an individual 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, DateTime from, DateTime to)
            => await GenerateSummaryAsync(personId, null, from, to);

        /// <summary>
        /// Summarise exercise records in a date range for an individual and activity
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int activityTypeId, DateTime from, DateTime to)
            => await GenerateSummaryAsync(personId, activityTypeId, from, to);

        /// <summary>
        /// Summarise exercise records for the last N days for an individual
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int days)
            => await GenerateSummaryAsync(personId, null, DateTime.Now.AddDays(-days + 1), DateTime.Now);

        /// <summary>
        /// Summarise exercise records for the last N days for an individual and activity
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int activityTypeId, int days)
            => await GenerateSummaryAsync(personId, activityTypeId, DateTime.Now.AddDays(-days + 1), DateTime.Now);

        /// <summary>
        /// Summarise the set of exercise measurements matching the specified criteria
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ExerciseSummary>> GenerateSummaryAsync(int personId, int? activityTypeId, DateTime from, DateTime to)
        {
            var measurements = await _factory.ExerciseMeasurements.ListAsync(x =>   (x.PersonId == personId) &&
                                                                                    ((activityTypeId == null) || (x.ActivityTypeId == activityTypeId.Value)) &&
                                                                                    (x.Date >= from) &&
                                                                                    (x.Date <= to),
                                                                                    1, int.MaxValue);

            var summaries = await Summarise(measurements);
            return summaries;
        }

        /// <summary>
        /// Generate a summary for a groupo of measurements for a single person and activity type
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        private static ExerciseSummary SummariseMeasurementGroup(IEnumerable<ExerciseMeasurement> measurements)
        {
            // If the activity type is consistent across all measurements, use that. Otherwise, set it to null
            int? activityTypeId = measurements.Select(x => x.ActivityTypeId).Distinct().Count() == 1 ? measurements.First().ActivityTypeId : null;

            // Create the summary
            var summary = new ExerciseSummary()
            {
                PersonId = measurements.First().PersonId,
                From = measurements.Min(x => x.Date),
                To = measurements.Max(x => x.Date),
                ActivityTypeId = activityTypeId,
                Count = measurements.Count(),
                TotalDuration = measurements.Sum(x => x.Duration),
                TotalDistance = measurements.Sum(x => x.Distance ?? 0M),
                TotalCalories = measurements.Sum(x => x.Calories),
                MinimumHeartRate = measurements.Where(x => x.MinimumHeartRate > 0).Min(x => x.MinimumHeartRate),
                MaximumHeartRate = measurements.Where(x => x.MaximumHeartRate > 0).Max(x => x.MaximumHeartRate)
            };

            return summary;
        }
    }
}