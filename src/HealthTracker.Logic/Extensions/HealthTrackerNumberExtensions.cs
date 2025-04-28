namespace HealthTracker.Logic.Extensions
{
    public static class HealthTrackerNumberExtensions
    {
        /// <summary>
        /// Convert a duration in seconds to HH:MM:SS format
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static string ToFormattedDuration(this int duration)
        {
            int hours = duration / 3600;
            int minutes = (duration - 3600 * hours) / 60;
            int seconds = duration - 3600 * hours - 60 * minutes;

            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }
}