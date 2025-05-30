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
    public class ImportController : Controller
    {
        private readonly IBackgroundQueue<PersonImportWorkItem> _personQueue;
        private readonly IBackgroundQueue<WeightMeasurementImportWorkItem> _weightMeasurementQueue;
        private readonly IBackgroundQueue<BloodPressureMeasurementImportWorkItem> _bloodPressureMeasurementQueue;
        private readonly IBackgroundQueue<OmronBloodPressureImportWorkItem> _omronBloodPressureMeasurementQueue;
        private readonly IBackgroundQueue<BloodGlucoseMeasurementImportWorkItem> _bloodGlucoseMeasurementQueue;
        private readonly IBackgroundQueue<CholesterolMeasurementImportWorkItem> _cholesterolMeasurementQueue;
        private readonly IBackgroundQueue<ExerciseMeasurementImportWorkItem> _exerciseMeasurementQueue;
        private readonly IBackgroundQueue<BloodOxygenSaturationMeasurementImportWorkItem> _spo2MeasurementQueue;
        private readonly IBackgroundQueue<BeverageConsumptionMeasurementImportWorkItem> _beverageConsumptionMeasurementQueue;


        public ImportController(
            IBackgroundQueue<PersonImportWorkItem> personQueue,
            IBackgroundQueue<WeightMeasurementImportWorkItem> weightMeasurementQueue,
            IBackgroundQueue<BloodPressureMeasurementImportWorkItem> bloodPressureMeasurementQueue,
            IBackgroundQueue<OmronBloodPressureImportWorkItem> omronBloodPressureMeasurementQueue,
            IBackgroundQueue<CholesterolMeasurementImportWorkItem> cholesterolMeasurementQueue,
            IBackgroundQueue<ExerciseMeasurementImportWorkItem> exerciseMeasurementQueue,
            IBackgroundQueue<BloodOxygenSaturationMeasurementImportWorkItem> spo2MeasurementQueue,
            IBackgroundQueue<BloodGlucoseMeasurementImportWorkItem> bloodGlucoseMeasurementQueue,
            IBackgroundQueue<BeverageConsumptionMeasurementImportWorkItem> beverageConsumptionMeasurementQueue)
        {
            _personQueue = personQueue;
            _weightMeasurementQueue = weightMeasurementQueue;
            _bloodPressureMeasurementQueue = bloodPressureMeasurementQueue;
            _omronBloodPressureMeasurementQueue = omronBloodPressureMeasurementQueue;
            _cholesterolMeasurementQueue = cholesterolMeasurementQueue;
            _exerciseMeasurementQueue = exerciseMeasurementQueue;
            _spo2MeasurementQueue = spo2MeasurementQueue;
            _bloodGlucoseMeasurementQueue = bloodGlucoseMeasurementQueue;
            _beverageConsumptionMeasurementQueue = beverageConsumptionMeasurementQueue;
        }

        [HttpPost]
        [Route("person")]
        public IActionResult ImportPeople([FromBody] PersonImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Person Import";

            // Queue the work item
            _personQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("weightmeasurement")]
        public IActionResult ImportWeightMeasurements([FromBody] WeightMeasurementImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Weight Measurement Import";

            // Queue the work item
            _weightMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("bloodpressuremeasurement")]
        public IActionResult ImportBloodPressureMeasurements([FromBody] BloodPressureMeasurementImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Blood Pressure Measurement Import";

            // Queue the work item
            _bloodPressureMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("omronbloodpressure")]
        public IActionResult ImportOmronBloodPressureMeasurements([FromBody] OmronBloodPressureImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "OMRON Blood Pressure Measurement Import";

            // Queue the work item
            _omronBloodPressureMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("cholesterolmeasurement")]
        public IActionResult ImportCholesterolMeasurements([FromBody] CholesterolMeasurementImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Cholesterol Measurement Import";

            // Queue the work item
            _cholesterolMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("exercisemeasurement")]
        public IActionResult ImportExerciseMeasurements([FromBody] ExerciseMeasurementImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Exercise Measurement Import";

            // Queue the work item
            _exerciseMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("bloodoxygensaturationmeasurement")]
        public IActionResult ImportBloodOxygenSaturationMeasurements([FromBody] BloodOxygenSaturationMeasurementImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Blood Oxygen Saturation Measurement Import";

            // Queue the work item
            _spo2MeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("bloodglucosemeasurement")]
        public IActionResult ImportBloodGlucoseMeasurements([FromBody] BloodGlucoseMeasurementImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Blood Glucose Measurement Import";

            // Queue the work item
            _bloodGlucoseMeasurementQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("beverageconsumptionmeasurement")]
        public IActionResult ImportBeverageConsumptionMeasurements([FromBody] BeverageConsumptionMeasurementImportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Beverage Consumption Measurement Import";

            // Queue the work item
            _beverageConsumptionMeasurementQueue.Enqueue(item);
            return Accepted();
        }
    }
}
