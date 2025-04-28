using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using HealthTracker.Entities.Medications;

namespace HealthTracker.Data
{
    [ExcludeFromCodeCoverage]
    public partial class HealthTrackerDbContext : DbContext
    {
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<ActivityType> ActivityTypes { get; set; }
        public virtual DbSet<WeightMeasurement> WeightMeasurements { get; set; }
        public virtual DbSet<BMIBand> BMIBands { get; set;}
        public virtual DbSet<BloodPressureMeasurement> BloodPressureMeasurements { get; set; }
        public virtual DbSet<BloodPressureBand> BloodPressureBands { get; set;}
        public virtual DbSet<BloodOxygenSaturationMeasurement> BloodOxygenSaturationMeasurements { get; set;}
        public virtual DbSet<BloodOxygenSaturationBand> BloodOxygenSaturationBands { get; set;}
        public virtual DbSet<ExerciseMeasurement> ExerciseMeasurements { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<CholesterolMeasurement> CholesterolMeasurements { get; set; }
        public virtual DbSet<Medication> Medications { get; set; }
        public virtual DbSet<PersonMedication> PersonMedications { get; set; }
        public virtual DbSet<BloodGlucoseMeasurement> BloodGlucoseMeasurements { get; set; }


        public HealthTrackerDbContext(DbContextOptions<HealthTrackerDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initialise the model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.UserName).IsRequired().HasColumnName("UserName");
                entity.Property(e => e.Password).IsRequired().HasColumnName("Password");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("PEOPLE");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.FirstNames).IsRequired().HasColumnName("firstnames").HasColumnType("VARCHAR(100)");
                entity.Property(e => e.Surname).IsRequired().HasColumnName("surname").HasColumnType("VARCHAR(100)");
                entity.Property(e => e.DateOfBirth).IsRequired().HasColumnName("dob").HasColumnType("DATETIME");
                entity.Property(e => e.Height).IsRequired().HasColumnName("height");
            });

            modelBuilder.Entity<ActivityType>(entity =>
            {
                entity.ToTable("ACTIVITY_TYPES");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Description).IsRequired().HasColumnName("description").HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<WeightMeasurement>(entity =>
            {
                entity.ToTable("WEIGHT");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.Weight).IsRequired().HasColumnName("weight");
            });

            modelBuilder.Entity<BloodPressureMeasurement>(entity =>
            {
                entity.ToTable("BLOOD_PRESSURE");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.Systolic).IsRequired().HasColumnName("systolic");
                entity.Property(e => e.Diastolic).IsRequired().HasColumnName("diastolic");
            });

            modelBuilder.Entity<CholesterolMeasurement>(entity =>
            {
                entity.ToTable("CHOLESTEROL");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.Total).IsRequired().HasColumnName("total");
                entity.Property(e => e.HDL).IsRequired().HasColumnName("hdl");
                entity.Property(e => e.LDL).IsRequired().HasColumnName("ldl");
                entity.Property(e => e.Triglycerides).IsRequired().HasColumnName("triglycerides");
            });

            modelBuilder.Entity<ExerciseMeasurement>(entity =>
            {
                entity.ToTable("EXERCISE");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.ActivityTypeId).HasColumnName("activity_id");
                entity.Property(e => e.Duration).IsRequired().HasColumnName("duration");
                entity.Property(e => e.Distance).HasColumnName("distance");
                entity.Property(e => e.Calories).IsRequired().HasColumnName("calories");
                entity.Property(e => e.MinimumHeartRate).IsRequired().HasColumnName("minimum_heart_rate");
                entity.Property(e => e.MaximumHeartRate).IsRequired().HasColumnName("maximum_heart_rate");
            });

            modelBuilder.Entity<BloodPressureBand>(entity =>
            {
                entity.ToTable("BLOOD_PRESSURE_BAND");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.MinimumSystolic).IsRequired().HasColumnName("minimum_systolic");
                entity.Property(e => e.MaximumSystolic).IsRequired().HasColumnName("maximum_systolic");
                entity.Property(e => e.MinimumDiastolic).IsRequired().HasColumnName("minimum_diastolic");
                entity.Property(e => e.MaximumDiastolic).IsRequired().HasColumnName("maximum_diastolic");
                entity.Property(e => e.Order).IsRequired().HasColumnName("order");
                entity.Property(e => e.MatchAll).IsRequired().HasColumnName("match_all");
            });

            modelBuilder.Entity<BMIBand>(entity =>
            {
                entity.ToTable("BMI_BAND");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.MinimumBMI).IsRequired().HasColumnName("minimum_bmi");
                entity.Property(e => e.MaximumBMI).IsRequired().HasColumnName("maximum_bmi");
                entity.Property(e => e.Order).IsRequired().HasColumnName("order");
            });

            modelBuilder.Entity<Medication>(entity =>
            {
                entity.ToTable("MEDICATION");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
            });

            modelBuilder.Entity<PersonMedication>(entity =>
            {
                entity.ToTable("PERSON_MEDICATION");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).IsRequired().HasColumnName("person_id");
                entity.Property(e => e.MedicationId).IsRequired().HasColumnName("medication_id");
                entity.Property(e => e.DailyDose).IsRequired().HasColumnName("daily_dose");
                entity.Property(e => e.Stock).IsRequired().HasColumnName("stock");
                entity.Property(e => e.LastTaken).HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
            });

            modelBuilder.Entity<BloodOxygenSaturationMeasurement>(entity =>
            {
                entity.ToTable("SPO2_MEASUREMENT");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).IsRequired().HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.Percentage).IsRequired().HasColumnName("percentage");
            });

            modelBuilder.Entity<BloodOxygenSaturationBand>(entity =>
            {
                entity.ToTable("SPO2_BAND");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.MinimumSPO2).IsRequired().HasColumnName("minimum_spo2");
                entity.Property(e => e.MaximumSPO2).IsRequired().HasColumnName("maximum_spo2");
                entity.Property(e => e.MinimumAge).IsRequired().HasColumnName("minimum_age");
                entity.Property(e => e.MaximumAge).IsRequired().HasColumnName("maximum_age");
            });

            modelBuilder.Entity<BloodGlucoseMeasurement>(entity =>
            {
                entity.ToTable("BLOOD_GLUCOSE");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.Level).HasColumnName("level");
            });
        }
    }
}