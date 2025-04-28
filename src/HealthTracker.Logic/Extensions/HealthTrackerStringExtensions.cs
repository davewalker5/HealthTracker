using HealthTracker.Entities.Exceptions;

namespace HealthTracker.Logic.Extensions
{
    public static class HealthTrackerStringExtensions
    {
        /// <summary>
        /// Extract systolic and diastolic blood pressure from a string representation expressed
        /// as systolic/diastolic
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        public static (int systolic, int diastolic) ToBloodPressureComponents(string measurement)
        {
            try
            {
                var words = measurement.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length != 2)
                {
                    throw new FormatException();
                }

                return (int.Parse(words[0]), int.Parse(words[1]));
            }
            catch (FormatException ex)
            {
                var message = $"'{measurement}' is not a valid blood pressure measurement";
                throw new InvalidMeasurementFormatException(message, ex);
            }
            catch (NullReferenceException ex)
            {
                var message = $"'null' is not a valid blood pressure measurement";
                throw new InvalidMeasurementFormatException(message, ex);
            }
        }

        /// <summary>
        /// Convert a duration in HH:MM:SS format to a duration in seconds
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static int ToDuration(this string duration)
        {
            try
            {
                var words = duration.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length != 3)
                {
                    throw new FormatException();
                }

                var hours = int.Parse(words[0]);
                var minutes = int.Parse(words[1]);
                var seconds = int.Parse(words[2]);

                return 3600 * hours + 60 * minutes + seconds;
            }
            catch (FormatException ex)
            {
                var message = $"'{duration}' is not a valid duration";
                throw new InvalidMeasurementFormatException(message, ex);
            }
            catch (NullReferenceException ex)
            {
                var message = $"'null' is not a valid duration";
                throw new InvalidMeasurementFormatException(message, ex);
            }
        }
    }
}
