using HealthTracker.Api.Entities;
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
    public class PersonMedicationController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public PersonMedicationController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a single association given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<PersonMedication>> Get(int id)
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == id, 1, int.MaxValue);
            await PopulateMedication(associations);
            _factory.MedicationActionGenerator.DetermineActions(associations);
            return associations.First();
        }

        /// <summary>
        /// Return a list of all associations for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<PersonMedication>>> ListPersonMedicationsAsync(int personId, int pageNumber, int pageSize)
        {
            // Retrieve the associations
            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == personId, pageNumber, pageSize);
            if (associations == null)
            {
                return NoContent();
            }

            // Populate the medication and calculate actions for each association
            await PopulateMedication(associations);
            _factory.MedicationActionGenerator.DetermineActions(associations);
            return associations;
        }

        /// <summary>
        /// Add an associations from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<PersonMedication>> AddPersonMedicationAsync([FromBody] PersonMedication template)
        {
            // Add the new association
            var association = await _factory.PersonMedications.AddAsync(
                template.PersonId,
                template.MedicationId,
                template.DailyDose,
                template.Stock,
                template.LastTaken
            );

            // Calculate actions based on association properties
            _factory.MedicationActionGenerator.DetermineActions(association);
            return association;
        }

        /// <summary>
        /// Update an association from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<PersonMedication>> UpdatePersonMedicationAsync([FromBody] PersonMedication template)
        {
            // Update the association
            var association = await _factory.PersonMedications.UpdateAsync(
                template.Id,
                template.PersonId,
                template.MedicationId,
                template.DailyDose,
                template.Stock,
                template.LastTaken,
                template.Active
            );

            // Calculate actions based on association properties
            _factory.MedicationActionGenerator.DetermineActions(association);
            return association;
        }

        [HttpPut]
        [Route("setstate")]
        public async Task<ActionResult<PersonMedication>> SetState([FromBody] PersonMedicationState state)
        {
            // Set the state of the association
            var association = (state.State) ?
                                    await _factory.PersonMedications.ActivateAsync(state.Id) :
                                    await _factory.PersonMedications.DeactivateAsync(state.Id);

            // Calculate actions based on association properties
            _factory.MedicationActionGenerator.DetermineActions(association);
            return association;
        }

        /// <summary>
        /// Delete an association
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePersonMedication(int id)
        {
            // Check the medication exists, first
            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (!associations.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.PersonMedications.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Populate the medication for each association in a collection of associations
        /// </summary>
        /// <param name="associations"></param>
        /// <returns></returns>
        private async Task PopulateMedication(IEnumerable<PersonMedication> associations)
        {
            var medications = await _factory.Medications.ListAsync(x => true, 1, int.MaxValue);
            foreach (var association in associations)
            {
                association.Medication = medications.First(x => x.Id == association.MedicationId);
            }
        }
    }
}
