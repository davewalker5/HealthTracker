using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Entities.Identity
{
    [ExcludeFromCodeCoverage]
    public class Person
    {
        [Key]
        public int Id { get ; set; }
        public string FirstNames { get ; set; }
        public string Surname { get ; set; }
        public DateTime DateOfBirth { get ; set; }
        public decimal Height { get; set; }
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

        public string Name()
            => $"{FirstNames} {Surname}";

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
