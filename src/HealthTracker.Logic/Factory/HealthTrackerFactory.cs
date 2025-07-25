using HealthTracker.Data;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Database;
using HealthTracker.Logic.Medications;
using HealthTracker.Configuration.Interfaces;
using Microsoft.EntityFrameworkCore;
using HealthTracker.Logic.Calculations;

namespace HealthTracker.Logic.Factory
{
    public class HealthTrackerFactory : IHealthTrackerFactory
    {
        private readonly Lazy<IPersonManager> _people;
        private readonly Lazy<IWeightMeasurementManager> _weightMeasurements;
        private readonly Lazy<IBMIBandManager> _bmiBands;
        private readonly Lazy<IBloodPressureMeasurementManager> _bloodPressureMeasurements;
        private readonly Lazy<IBloodGlucoseMeasurementManager> _gloodGlucoseMeasurements;
        private readonly Lazy<IBloodOxygenSaturationMeasurementManager> _spo2Measurements;
        private readonly Lazy<IBloodPressureBandManager> _bloodPressureBands;
        private readonly Lazy<IBloodOxygenSaturationBandManager> _spo2Bands;
        private readonly Lazy<ICholesterolMeasurementManager> _cholesterolMeasurements;
        private readonly Lazy<IActivityTypeManager> _activityTypes;
        private readonly Lazy<IExerciseMeasurementManager> _exerciseMeasurements;
        private readonly Lazy<IExerciseCalculator> _exerciseCalculator;
        private readonly Lazy<IUserManager> _users;
        private readonly Lazy<IJobStatusManager> _jobStatus;

        private readonly Lazy<IBeverageManager> _beverages;
        private readonly Lazy<IFoodSourceManager> _foodSources;
        private readonly Lazy<IFoodCategoryManager> _foodCategories;
        private readonly Lazy<IBeverageConsumptionMeasurementManager> _beverageConsumption;
        private readonly Lazy<IBeverageMeasureManager> _beverageMeasures;
        private readonly Lazy<INutritionalValueManager> _nutritionalValues;
        private readonly Lazy<IFoodItemManager> _foodItems;
        private readonly Lazy<IMealManager> _meals;
        private readonly Lazy<IMealConsumptionMeasurementManager> _mealConsumption;
        private readonly Lazy<IMealFoodItemManager> _mealFoodItems;

        private readonly Lazy<IMedicationManager> _medications;
        private readonly Lazy<IPersonMedicationManager> _personMedications;
        private readonly Lazy<IMedicationStockUpdater> _stockUpdater;

        private readonly Lazy<IBloodPressureCalculator> _bloodPressureCalculator;
        private readonly Lazy<IBloodPressureAssessor> _bloodPressureAssessor;
        private readonly Lazy<IBloodOxygenSaturationCalculator> _spo2Calculator;
        private readonly Lazy<IBloodOxygenSaturationAssessor> _spo2Assessor;
        private readonly Lazy<IWeightCalculator> _weightCalculator;
        private readonly Lazy<IMedicationActionGenerator> _actionGenerator;

        private readonly Lazy<IAlcoholUnitsCalculator> _alcoholUnitsCalculator;
        private readonly Lazy<IBeverageConsumptionCalculator> _beverageConsumptionCalculator;

        public DbContext Context { get; private set; }
        public IHealthTrackerLogger Logger { get; private set; }
        public IPersonManager People { get { return _people.Value; } }
        public IWeightMeasurementManager WeightMeasurements { get { return _weightMeasurements.Value; } }
        public IBMIBandManager BMIBands { get { return _bmiBands.Value; } }
        public IBloodPressureMeasurementManager BloodPressureMeasurements { get { return _bloodPressureMeasurements.Value; } }
        public IBloodGlucoseMeasurementManager BloodGlucoseMeasurements { get { return _gloodGlucoseMeasurements.Value; } }
        public IBloodOxygenSaturationMeasurementManager BloodOxygenSaturationMeasurements { get { return _spo2Measurements.Value; } }
        public IBloodPressureBandManager BloodPressureBands { get { return _bloodPressureBands.Value; } }
        public IBloodOxygenSaturationBandManager BloodOxygenSaturationBands { get { return _spo2Bands.Value; } }
        public IBloodPressureAssessor BloodPressureAssessor { get { return _bloodPressureAssessor.Value; } }
        public IBloodOxygenSaturationAssessor BloodOxygenSaturationAssessor { get { return _spo2Assessor.Value; } }
        public ICholesterolMeasurementManager CholesterolMeasurements { get { return _cholesterolMeasurements.Value; } }
        public IActivityTypeManager ActivityTypes { get { return _activityTypes.Value; } }
        public IExerciseMeasurementManager ExerciseMeasurements { get { return _exerciseMeasurements.Value; } }
        public IExerciseCalculator ExerciseCalculator { get { return _exerciseCalculator.Value; } }
        public IUserManager Users { get { return _users.Value; } }
        public IJobStatusManager JobStatuses { get { return _jobStatus.Value; } }

        public IBeverageManager Beverages { get { return _beverages.Value; } }
        public IFoodSourceManager FoodSources { get { return _foodSources.Value; } }
        public IFoodCategoryManager FoodCategories { get { return _foodCategories.Value; } }
        public IBeverageConsumptionMeasurementManager BeverageConsumptionMeasurements { get { return _beverageConsumption.Value; } }
        public IBeverageMeasureManager BeverageMeasures { get { return _beverageMeasures.Value; } }
        public INutritionalValueManager NutritionalValues { get { return _nutritionalValues.Value; } }
        public IFoodItemManager FoodItems { get { return _foodItems.Value; } }
        public IMealManager Meals { get { return _meals.Value; } }
        public IMealConsumptionMeasurementManager MealConsumptionMeasurements { get { return _mealConsumption.Value; } }
        public IMealFoodItemManager MealFoodItems { get { return _mealFoodItems.Value; } }

        public IMedicationManager Medications { get { return _medications.Value; } }
        public IPersonMedicationManager PersonMedications { get { return _personMedications.Value;} }
        public IMedicationStockUpdater MedicationStockUpdater { get { return _stockUpdater.Value; } }
        public IMedicationActionGenerator MedicationActionGenerator { get { return _actionGenerator.Value; }}

        public IBloodPressureCalculator BloodPressureCalculator { get { return _bloodPressureCalculator.Value; } }
        public IBloodOxygenSaturationCalculator BloodOxygenSaturationCalculator { get { return _spo2Calculator.Value;} }
        public IWeightCalculator WeightCalculator { get { return _weightCalculator.Value; } }

        public IAlcoholUnitsCalculator AlcoholUnitsCalculator { get { return _alcoholUnitsCalculator.Value; } }
        public IBeverageConsumptionCalculator BeverageConsumptionCalculator { get { return _beverageConsumptionCalculator.Value; } }

        public HealthTrackerFactory(
            HealthTrackerDbContext context,
            IHealthTrackerApplicationSettings settings,
            IHealthTrackerLogger logger)
        {
            Context = context;
            Logger = logger;

            _people = new Lazy<IPersonManager>(() => new PersonManager(this));
            _weightMeasurements = new Lazy<IWeightMeasurementManager>(() => new WeightMeasurementManager(this));
            _bmiBands = new Lazy<IBMIBandManager>(() => new BMIBandManager(this));
            _bloodPressureMeasurements = new Lazy<IBloodPressureMeasurementManager>(() => new BloodPressureMeasurementManager(this));
            _spo2Measurements = new Lazy<IBloodOxygenSaturationMeasurementManager>(() => new BloodOxygenSaturationMeasurementManager(this));
            _gloodGlucoseMeasurements = new Lazy<IBloodGlucoseMeasurementManager>(() => new BloodGlucoseMeasurementManager(this));
            _bloodPressureBands = new Lazy<IBloodPressureBandManager>(() => new BloodPressureBandManager(this));
            _spo2Bands = new Lazy<IBloodOxygenSaturationBandManager>(() => new BloodOxygenSaturationBandManager(this));
            _cholesterolMeasurements = new Lazy<ICholesterolMeasurementManager>(() => new CholesterolMeasurementManager(this));
            _activityTypes = new Lazy<IActivityTypeManager>(() => new ActivityTypeManager(this));
            _exerciseMeasurements = new Lazy<IExerciseMeasurementManager>(() => new ExerciseMeasurementManager(this));
            _exerciseCalculator =  new Lazy<IExerciseCalculator>(() => new ExerciseCalculator(this));
            _users = new Lazy<IUserManager>(() => new UserManager(this));
            _jobStatus = new Lazy<IJobStatusManager>(() => new JobStatusManager(this));

            _beverages = new Lazy<IBeverageManager>(() => new BeverageManager(this));
            _foodSources = new Lazy<IFoodSourceManager>(() => new FoodSourceManager(this));
            _foodCategories = new Lazy<IFoodCategoryManager>(() => new FoodCategoryManager(this));;
            _beverageConsumption = new Lazy<IBeverageConsumptionMeasurementManager>(() => new BeverageConsumptionMeasurementManager(this));
            _beverageMeasures = new Lazy<IBeverageMeasureManager>(() => new BeverageMeasureManager(this));
            _nutritionalValues = new Lazy<INutritionalValueManager>(() => new NutritionalValueManager(this));
            _foodItems = new Lazy<IFoodItemManager>(() => new FoodItemManager(this));
            _meals = new Lazy<IMealManager>(() => new MealManager(this));
            _mealConsumption = new Lazy<IMealConsumptionMeasurementManager>(() => new MealConsumptionMeasurementManager(this));
            _mealFoodItems = new Lazy<IMealFoodItemManager>(() => new MealFoodItemManager(this));

            _medications = new Lazy<IMedicationManager>(() => new MedicationManager(this));
            _personMedications = new Lazy<IPersonMedicationManager>(() => new PersonMedicationManager(this));
            _stockUpdater = new Lazy<IMedicationStockUpdater>(() => new MedicationStockUpdater(this));
            _actionGenerator = new Lazy<IMedicationActionGenerator>(() => new MedicationActionGenerator(settings));

            _bloodPressureCalculator = new Lazy<IBloodPressureCalculator>(() => new BloodPressureCalculator(this));
            _bloodPressureAssessor = new Lazy<IBloodPressureAssessor>(() => new BloodPressureAssessor(this));
            _spo2Calculator = new Lazy<IBloodOxygenSaturationCalculator>(() => new BloodOxygenSaturationCalculator(this));
            _spo2Assessor = new Lazy<IBloodOxygenSaturationAssessor>(() => new BloodOxygenSaturationAssessor(this));
            _weightCalculator = new Lazy<IWeightCalculator>(() => new WeightCalculator(this));

            _alcoholUnitsCalculator = new Lazy<IAlcoholUnitsCalculator>(() => new AlcoholUnitsCalculator());
            _beverageConsumptionCalculator = new Lazy<IBeverageConsumptionCalculator>(() => new BeverageConsumptionCalculator(this));
        }
    }
}
