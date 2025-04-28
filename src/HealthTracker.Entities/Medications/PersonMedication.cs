using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Medications
{
    [ExcludeFromCodeCoverage]
    public class PersonMedication
    {
        [Key]
        public int Id { get ; set; }
        public int PersonId { get; set; }
        public int MedicationId { get; set; }
        public int DailyDose { get; set; }
        public int Stock { get; set; }
        public DateTime? LastTaken { get; set; }
        public bool Active { get; set; }

        [NotMapped]
        public List<string> Actions { get; set; }

        public int DaysRemaining()
            => Stock / DailyDose;
        
        /// <summary>
        /// Return the date on which the curent stock will run out
        /// </summary>
        /// <returns></returns>
        public DateTime? LastDay()
        {
            DateTime? lastDay = null;

            if (LastTaken != null)
            {
                DateTime lastTaken = LastTaken ?? DateTime.Now;
                lastDay = new DateTime(lastTaken.Year, lastTaken.Month, lastTaken.Day, 0, 0, 0).AddDays(DaysRemaining());
            }

            return lastDay;
        }

        /// <summary>
        /// Return the suggested date for ordering more stock
        /// </summary>
        /// <param name="orderLeadTimeDays"></param>
        /// <returns></returns>
        public DateTime? OrderMoreDate(int orderLeadTimeDays)
        {
            DateTime? orderMoreDate = null;

            // Calculate the date the current stock will run out
            var lastDay = LastDay();
            if (lastDay != null)
            {
                orderMoreDate = lastDay.Value.AddDays(-orderLeadTimeDays);
            }

            return orderMoreDate;
        }
    }
}