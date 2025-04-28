using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Medications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class MedicationController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public MedicationController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return medication details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Medication>> GetMedicationByIdAsync(int id)
        {
            var medication = await _factory.Medications.GetAsync(x => x.Id == id);

            if (medication == null)
            {
                return NotFound();
            }

            return medication;
        }

        /// <summary>
        /// Return a list of all medications in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Medication>>> ListAllMedicationsAsync()
        {
            var medications = await _factory.Medications.ListAsync(x => true);

            if (medications == null)
            {
                return NoContent();
            }

            return medications;
        }

        /// <summary>
        /// Add a medication from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Medication>> AddMedicationAsync([FromBody] Medication template)
        {
            var medication = await _factory.Medications.AddAsync(template.Name);
            return medication;
        }

        /// <summary>
        /// Update a medication from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Medication>> UpdateMedicationAsync([FromBody] Medication template)
        {
            var medication = await _factory.Medications.UpdateAsync(template.Id, template.Name);
            return medication;
        }

        /// <summary>
        /// Delete a medication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            // Check the medication exists, first
            var medication = await _factory.Medications.GetAsync(x => x.Id == id);
            if (medication == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.Medications.DeleteAsync(id);
            return Ok();
        }
    }
}
