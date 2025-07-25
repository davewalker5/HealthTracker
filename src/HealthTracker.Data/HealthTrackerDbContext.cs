using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Measurements;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using HealthTracker.Entities.Medications;
using HealthTracker.Entities.Logging;

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
        public virtual DbSet<JobStatus> JobStatuses { get; set; }
        public virtual DbSet<Beverage> Beverages { get; set; }
        public virtual DbSet<FoodSource> FoodSources { get; set; }
        public virtual DbSet<FoodCategory> FoodCategories { get; set; }
        public virtual DbSet<BeverageConsumptionMeasurement> BeverageConsumptionMeasurements { get; set; }
        public virtual DbSet<BeverageMeasure> BeverageMeasures { get; set; }
        public virtual DbSet<NutritionalValue> NutritionalValues { get; set; }
        public virtual DbSet<FoodItem> FoodItems { get; set; }
        public virtual DbSet<Meal> Meals { get; set; }
        public virtual DbSet<MealConsumptionMeasurement> MealConsumptionMeasurements { get; set; }
        public virtual DbSet<MealFoodItem> MealFoodItems { get; set; }


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
                entity.Property(e => e.DistanceBased).HasColumnName("distance_based");
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

            modelBuilder.Entity<JobStatus>(entity =>
            {
                entity.ToTable("JOB_STATUS");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.Property(e => e.Parameters).HasColumnName("parameters");
                entity.Property(e => e.Start).IsRequired().HasColumnName("start").HasColumnType("DATETIME");
                entity.Property(e => e.End).HasColumnName("end").HasColumnType("DATETIME");
                entity.Property(e => e.Error).HasColumnName("error");
            });

            modelBuilder.Entity<Beverage>(entity =>
            {
                entity.ToTable("BEVERAGES");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasColumnType("VARCHAR(100)");
                entity.Property(e => e.TypicalABV).HasColumnName("typical_abv");
                entity.Property(e => e.IsHydrating).HasColumnName("is_hydrating");
                entity.Property(e => e.IsAlcohol).HasColumnName("is_alcohol");
            });

            modelBuilder.Entity<BeverageConsumptionMeasurement>(entity =>
            {
                entity.ToTable("BEVERAGE_CONSUMPTION");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.BeverageId).HasColumnName("beverage_id");
                entity.Property(e => e.Quantity).IsRequired().HasColumnName("quantity");
                entity.Property(e => e.Volume).IsRequired().HasColumnName("volume");
                entity.Property(e => e.ABV).IsRequired().HasColumnName("abv");
            });

            modelBuilder.Entity<BeverageMeasure>(entity =>
            {
                entity.ToTable("BEVERAGE_MEASURES");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.Property(e => e.Volume).IsRequired().HasColumnName("volume");
            });

            modelBuilder.Entity<FoodSource>(entity =>
            {
                entity.ToTable("FOOD_SOURCES");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<FoodCategory>(entity =>
            {
                entity.ToTable("FOOD_CATEGORIES");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<NutritionalValue>(entity =>
            {
                entity.ToTable("NUTRITIONAL_VALUES");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Calories).HasColumnName("calories");
                entity.Property(e => e.Fat).HasColumnName("fat");
                entity.Property(e => e.SaturatedFat).HasColumnName("saturated_fat");
                entity.Property(e => e.Protein).HasColumnName("protein");
                entity.Property(e => e.Carbohydrates).HasColumnName("carbohydrates");
                entity.Property(e => e.Sugar).HasColumnName("sugar");
                entity.Property(e => e.Fibre).HasColumnName("fibre");
            });

            modelBuilder.Entity<FoodItem>(entity =>
            {
                entity.ToTable("FOOD_ITEMS");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasColumnType("VARCHAR(100)");
                entity.Property(e => e.Portion).IsRequired().HasColumnName("portion");
                entity.Property(e => e.FoodCategoryId).IsRequired().HasColumnName("food_category_id");
                entity.Property(e => e.NutritionalValueId).HasColumnName("nutritional_value_id");

                entity.HasOne(e => e.FoodCategory)
                    .WithMany()
                    .HasForeignKey(e => e.FoodCategoryId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NutritionalValue)
                    .WithOne()
                    .HasForeignKey<FoodItem>(e => e.NutritionalValueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Meal>(entity =>
            {
                entity.ToTable("MEALS");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasColumnType("VARCHAR(100)");
                entity.Property(e => e.Portions).IsRequired().HasColumnName("portions");
                entity.Property(e => e.FoodSourceId).HasColumnName("food_source_id");
                entity.Property(e => e.NutritionalValueId).HasColumnName("nutritional_value_id");

                entity.HasOne(e => e.FoodSource)
                    .WithMany()
                    .HasForeignKey(e => e.FoodSourceId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NutritionalValue)
                    .WithOne()
                    .HasForeignKey<Meal>(e => e.NutritionalValueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MealConsumptionMeasurement>(entity =>
            {
                entity.ToTable("MEAL_CONSUMPTION");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PersonId).HasColumnName("person_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.MealId).HasColumnName("meal_id");
                entity.Property(e => e.Quantity).IsRequired().HasColumnName("quantity");
                entity.Property(e => e.NutritionalValueId).HasColumnName("nutritional_value_id");

                entity.HasOne(e => e.Meal)
                    .WithMany()
                    .HasForeignKey(e => e.MealId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NutritionalValue)
                    .WithOne()
                    .HasForeignKey<MealConsumptionMeasurement>(e => e.NutritionalValueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MealFoodItem>(entity =>
            {
                entity.ToTable("MEAL_FOOD_ITEMS");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.MealId).IsRequired().HasColumnName("meal_id");
                entity.Property(e => e.FoodItemId).IsRequired().HasColumnName("food_item_id");
                entity.Property(e => e.Quantity).IsRequired().HasColumnName("quantity");
                entity.Property(e => e.NutritionalValueId).HasColumnName("nutritional_value_id");

                modelBuilder.Entity<MealFoodItem>()
                        .HasOne<Meal>()
                        .WithMany(e => e.MealFoodItems)
                        .HasForeignKey(e => e.MealId)
                        .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<MealFoodItem>()
                    .HasOne(e => e.FoodItem)
                    .WithMany()
                    .HasForeignKey(e => e.FoodItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NutritionalValue)
                    .WithOne()
                    .HasForeignKey<MealFoodItem>(e => e.NutritionalValueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}