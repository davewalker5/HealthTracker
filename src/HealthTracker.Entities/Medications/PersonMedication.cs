using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Entities.Identity;

namespace HealthTracker.Entities.Medications
{
    [ExcludeFromCodeCoverage]
    public class PersonMedication : HealthTrackerEntityBase
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Person")]
        [Required(ErrorMessage = "You must select a person")]
        [ForeignKey(nameof(Person))]
        public int PersonId { get; set; }

        [DisplayName("Medication")]
        [Required(ErrorMessage = "You must select a medication")]
        [ForeignKey(nameof(Medication))]
        public int MedicationId { get; set; }

        [DisplayName("Daily Dose")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int DailyDose { get; set; }

        [DisplayName("Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int Stock { get; set; }

        [DisplayName("Last Taken")]
        public DateTime? LastTaken { get; set; }

        [DisplayName("Active")]
        public bool Active { get; set; }

        [NotMapped]
        public Medication Medication { get; set; }

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