namespace HealthTracker.Logic.Extensions
{
    public static class HealthTrackerDateExtensions
    {
        /// <summary>
        /// Return a date with the time components set to 0
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime DateWithoutTime(DateTime date)
            => new(date.Year, date.Month, date.Day, 0, 0, 0, 0);

        /// <summary>
        /// Return todays date with the time components set to 0
        /// </summary>
        public static DateTime TodayWithoutTime()
            => DateWithoutTime(DateTime.Now);
    }
}