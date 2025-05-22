namespace HealthTracker.Mvc.Models
{
    public abstract class ReferenceDataListViewModel
    {
        /// <summary>
        /// Return a string representation of an integer value that replaces "MaxValue" with a
        /// "no value" placeholder
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetFormattedValue(int value)
            => value == int.MaxValue ? "-" : value.ToString();

        /// <summary>
        /// Return a string representation of a decimalvalue that replaces "MaxValue" with a
        /// "no value" placeholder
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetFormattedValue(decimal value)
            => value == decimal.MaxValue ? "-" : value.ToString();
    }
}