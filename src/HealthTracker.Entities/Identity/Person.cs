using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Entities.Identity
{
    [ExcludeFromCodeCoverage]
    public class Person : HealthTrackerEntityBase
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("First Names")]
        [Required(ErrorMessage = "You must provide first names")]
        public string FirstNames { get ; set; }

        [DisplayName("Surname")]
        [Required(ErrorMessage = "You must provide a surname")]
        public string Surname { get ; set; }

        [DisplayName("Date of Birth")]
        [UIHint("DatePicker")]
        [Required(ErrorMessage = "You must provide a date of birth")]
        public DateTime DateOfBirth { get ; set; }

        [DisplayName("Height")]
        [Range(1.0, double.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal Height { get; set; }

        [DisplayName("Gender")]
        public Gender Gender { get; set; } = Gender.Unspecified;

        [NotMapped]
        public DistanceUnit DistanceUnits { get; private set; } = DistanceUnit.Kilometres;

        [NotMapped]
        public HeightUnit HeightUnits { get; private set; } = HeightUnit.Metres;

        [NotMapped]
        public WeightUnit WeightUnits { get; private set; } = WeightUnit.Kilograms;

        [NotMapped]
        public BloodPressureUnit BloodPressureUnits { get; private set; } = BloodPressureUnit.MillimetresOfMercury;

        [NotMapped]
        public HeartRateUnit HeartRateUnits { get; private set; } = HeartRateUnit.BeatsPerMinute;

        [NotMapped]
        public CholesterolUnit CholesterolUnits { get; private set; } = CholesterolUnit.MillimolesPerLitre;

        public string Name { get { return $"{FirstNames} {Surname}"; } }

        /// <summary>
        /// Return the persons age in years
        /// </summary>
        /// <returns></returns>
        public int AgeInYears()
        {
            // Calculate the "raw" value
            var now = DateTime.Now;
            int age = now.Year - DateOfBirth.Year;

            // Account for leap years
            if ((now.Month < DateOfBirth.Month) || ((now.Month == DateOfBirth.Month) && (now.Day < DateOfBirth.Day)))
            {
                age--;
            }

            return age;
        }
    }
}
