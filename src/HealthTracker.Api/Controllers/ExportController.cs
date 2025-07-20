using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ExportController : Controller
    {
        private readonly IBackgroundQueue<PersonExportWorkItem> _personQueue;
        private readonly IBackgroundQueue<WeightMeasurementExportWorkItem> _weightMeasurementQueue;
        private readonly IBackgroundQueue<BloodPressureMeasurementExportWorkItem> _bloodPressureMeasurementQueue;
        private readonly IBackgroundQueue<BloodGlucoseMeasurementExportWorkItem> _bloodGlucoseMeasurementQueue;
        private readonly IBackgroundQueue<DailyAverageBloodPressureExportWorkItem> _dailyAverageBloodPressureQueue;
        private readonly IBackgroundQueue<CholesterolMeasurementExportWorkItem> _cholesterolMeasurementQueue;
        private readonly IBackgroundQueue<ExerciseMeasurementExportWorkItem> _exerciseMeasurementQueue;
        private readonly IBackgroundQueue<BloodOxygenSaturationMeasurementExportWorkItem> _spo2MeasurementQueue;
        private readonly IBackgroundQueue<DailyAverageBloodOxygenSaturationExportWorkItem> _dailyAverageSPO2Queue;
        private readonly IBackgroundQueue<BeverageConsumptionMeasurementExportWorkItem> _beverageConsumptionMeasurementQueue;
        private readonly IBackgroundQueue<FoodItemExportWorkItem> _foodItemQueue;
        private readonly IBackgroundQueue<MealExportWorkItem> _mealQueue;
        private readonly IBackgroundQueue<MealConsumptionMeasurementExportWorkItem> _mealConsumptionQueue;

        public ExportController(
            IBackgroundQueue<PersonExportWorkItem> personQueue,
            IBackgroundQueue<WeightMeasurementExportWorkItem> weightMeasurementQueue,
            IBackgroundQueue<BloodPressureMeasurementExportWorkItem> bloodPressureMeasurementQueue,
            IBackgroundQueue<DailyAverageBloodPressureExportWorkItem> dailyAverageBloodPressureQueue,
            IBackgroundQueue<CholesterolMeasurementExportWorkItem> cholesterolMeasurementQueue,
            IBackgroundQueue<ExerciseMeasurementExportWorkItem> exerciseMeasurementQueue,
            IBackgroundQueue<BloodOxygenSaturationMeasurementExportWorkItem> spo2MeasurementQueue,
            IBackgroundQueue<DailyAverageBloodOxygenSaturationExportWorkItem> dailyAverageSPO2Queue,
            IBackgroundQueue<BloodGlucoseMeasurementExportWorkItem> bloodGlucoseMeasurementQueue,
            IBackgroundQueue<BeverageConsumptionMeasurementExportWorkItem> beverageConsumptionMeasurementQueue,
            IBackgroundQueue<FoodItemExportWorkItem> foodItemQueue,
            IBackgroundQueue<MealExportWorkItem> mealQueue,
            IBackgroundQueue<MealConsumptionMeasurementExportWorkItem> mealConsumptionQueue)
        {
            _personQueue = personQueue;
            _weightMeasurementQueue = weightMeasurementQueue;
            _bloodPressureMeasurementQueue = bloodPressureMeasurementQueue;
            _dailyAverageBloodPressureQueue = dailyAverageBloodPressureQueue;
            _cholesterolMeasurementQueue = cholesterolMeasurementQueue;
            _exerciseMeasurementQueue = exerciseMeasurementQueue;
            _spo2MeasurementQueue = spo2MeasurementQueue;
            _dailyAverageSPO2Queue = dailyAverageSPO2Queue;
            _bloodGlucoseMeasurementQueue = bloodGlucoseMeasurementQueue;
            _beverageConsumptionMeasurementQueue = beverageConsumptionMeasurementQueue;
            _foodItemQueue = foodItemQueue;
            _mealQueue = mealQueue;
            _mealConsumptionQueue = mealConsumptionQueue;
        }

        [HttpPost]
        [Route("person")]
        public IActionResult ExportPeople([FromBody] PersonExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Person Export";

            // Queue the work item
            _personQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("weightmeasurement")]
        public IActionResult ExportWeightMeasurements([FromBody] WeightMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Weight Measurement Export";

            // Queue the work item
            _weightMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("bloodpressuremeasurement")]
        public IActionResult ExportBloodPressureMeasurements([FromBody] BloodPressureMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Blood Pressure Measurement Export";

            // Queue the work item
            _bloodPressureMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("dailyaveragebloodpressure")]
        public IActionResult ExportDailyAverageBloodPressure([FromBody] DailyAverageBloodPressureExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Daily Average Blood Pressure Measurement Export";

            // Queue the work item
            _dailyAverageBloodPressureQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("cholesterolmeasurement")]
        public IActionResult ExportCholesterolMeasurements([FromBody] CholesterolMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Cholesterol Measurement Export";

            // Queue the work item
            _cholesterolMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("exercisemeasurement")]
        public IActionResult ExportExerciseMeasurements([FromBody] ExerciseMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Exercise Measurement Export";

            // Queue the work item
            _exerciseMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("bloodoxygensaturationmeasurement")]
        public IActionResult ExportBloodOxygenSaturationMeasurements([FromBody] BloodOxygenSaturationMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Blood Oxygen Saturation Measurement Export";

            // Queue the work item
            _spo2MeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("dailyaveragebloodoxygensaturation")]
        public IActionResult ExportDailyAverageBloodOxygenSaturation([FromBody] DailyAverageBloodOxygenSaturationExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Daily Average Blood Oxygen Saturation Measurement Export";

            // Queue the work item
            _dailyAverageSPO2Queue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("bloodglucosemeasurement")]
        public IActionResult ExportBloodGlucoseMeasurements([FromBody] BloodGlucoseMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Blood Glucose Measurement Export";

            // Queue the work item
            _bloodGlucoseMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("beverageconsumptionmeasurement")]
        public IActionResult ExportBeverageConsumptionMeasurements([FromBody] BeverageConsumptionMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Beverage Consumption Measurement Export";

            // Queue the work item
            _beverageConsumptionMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("fooditem")]
        public IActionResult ExportFoodItems([FromBody] FoodItemExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Food Item Export";

            // Queue the work item
            _foodItemQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("meal")]
        public IActionResult ExportMeals([FromBody] MealExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Meal Export";

            // Queue the work item
            _mealQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("mealconsumptionmeasurement")]
        public IActionResult ExportMealConsumptionMeasurements([FromBody] MealConsumptionMeasurementExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Meal Consumption Measurement Export";

            // Queue the work item
            _mealConsumptionQueue.Enqueue(item);
            return Accepted();
        }
    }
}
