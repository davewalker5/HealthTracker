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
    public class MedicationTrackingController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public MedicationTrackingController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Add stock to the medication for a person
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("addstock")]
        public async Task<ActionResult<PersonMedication>> AddStockAsync([FromBody] MedicationTablets stock)
        {
            // Retrieve the person/medication association
            var association = await GetAssociation(stock.PersonId, stock.MedicationId);
            if (association == null)
            {
                return NotFound();
            }

            // Add the tablets to the stock
            association = await _factory.MedicationStockUpdater.AddStockAsync(association.Id, stock.Tablets);
            return association;
        }

        /// <summary>
        /// Set the stock level for a medication for a person
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("setstock")]
        public async Task<ActionResult<PersonMedication>> SetStockAsync([FromBody] MedicationTablets stock)
        {
            // Retrieve the person/medication association
            var association = await GetAssociation(stock.PersonId, stock.MedicationId);
            if (association == null)
            {
                return NotFound();
            }

            // Set the stock level
            association = await _factory.MedicationStockUpdater.SetStockAsync(association.Id, stock.Tablets);
            return association;
        }

        /// <summary>
        /// Set the daily dose level for a medication for a person
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("setdose")]
        public async Task<ActionResult<PersonMedication>> SetDoseAsync([FromBody] MedicationTablets dose)
        {
            // Retrieve the person/medication association
            var association = await GetAssociation(dose.PersonId, dose.MedicationId);
            if (association == null)
            {
                return NotFound();
            }

            // Set the daily dose
            association = await _factory.PersonMedications.SetDoseAsync(association.Id, dose.Tablets);
            return association;
        }

        /// <summary>
        /// Take 1 or more doses of a single medication associated with a person
        /// </summary>
        /// <param name="dose"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("take")]
        public async Task<ActionResult<PersonMedication>> TakeDoseAsync([FromBody] MedicationAssociation dose)
        {
            // Retrieve the person/medication association
            var association = await GetAssociation(dose.PersonId, dose.MedicationId);
            if (association == null)
            {
                return NotFound();
            }

            // Take a dose
            association = await _factory.MedicationStockUpdater.DecrementAsync(association.Id, 1);
            return association;
        }

        /// <summary>
        /// Take 1 or more doses of a all medications associated with a person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("takeall")]
        public async Task<IActionResult> TakeAllAsync([FromBody] PersonIdentifier person)
        {
            await _factory.MedicationStockUpdater.DecrementAllAsync(person.PersonId, 1);
            return Ok();
        }

        /// <summary>
        /// Un-take 1 or more doses of a single medication associated with a person
        /// </summary>
        /// <param name="dose"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("untake")]
        public async Task<ActionResult<PersonMedication>> UntakeDoseAsync([FromBody] MedicationAssociation dose)
        {
            // Retrieve the person/medication association
            var association = await GetAssociation(dose.PersonId, dose.MedicationId);
            if (association == null)
            {
                return NotFound();
            }

            // Take a dose
            association = await _factory.MedicationStockUpdater.IncrementAsync(association.Id, 1);
            return association;
        }

        /// <summary>
        /// Un-take 1 or more doses of all medications associated with a person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("untakeall")]
        public async Task<IActionResult> UnakeAllAsync([FromBody] PersonIdentifier person)
        {
            await _factory.MedicationStockUpdater.IncrementAllAsync(person.PersonId, 1);
            return Ok();
        }

        /// <summary>
        /// Fast-forward the stock of a single medication associated with a person
        /// </summary>
        /// <param name="dose"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("fastforward")]
        public async Task<ActionResult<PersonMedication>> FastForwardAsync([FromBody] MedicationAssociation dose)
        {
            // Retrieve the person/medication association
            var association = await GetAssociation(dose.PersonId, dose.MedicationId);
            if (association == null)
            {
                return NotFound();
            }

            // Take a dose
            association = await _factory.MedicationStockUpdater.FastForwardAsync(association.Id);
            return association;
        }

        /// <summary>
        /// Fast-forward the stock of all medications associated with a person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("fastforwardall")]
        public async Task<ActionResult<PersonMedication>> FastForwardAllAsync([FromBody] PersonIdentifier person)
        {
            await _factory.MedicationStockUpdater.FastForwardAllAsync(person.PersonId);
            return Ok();
        }

        /// <summary>
        /// Skip a dose of a single medication associated with a person
        /// </summary>
        /// <param name="dose"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("skip")]
        public async Task<ActionResult<PersonMedication>> SkipDoseAsync([FromBody] MedicationAssociation dose)
        {
            // Retrieve the person/medication association
            var association = await GetAssociation(dose.PersonId, dose.MedicationId);
            if (association == null)
            {
                return NotFound();
            }

            // Take a dose
            association = await _factory.MedicationStockUpdater.SkipAsync(association.Id);
            return association;
        }

        /// <summary>
        /// Skip a dose of all medications associated with a person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("skipall")]
        public async Task<IActionResult> SkipAllAsync([FromBody] PersonIdentifier person)
        {
            await _factory.MedicationStockUpdater.SkipAllAsync(person.PersonId);
            return Ok();
        }

        /// <summary>
        /// Retrieve the person/medication association for a person and medication
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <returns></returns>
        private async Task<PersonMedication> GetAssociation(int personId, int medicationId)
        {
            var associations = await _factory.PersonMedications
                                             .ListAsync(x =>
                                                (x.PersonId == personId) &&
                                                (x.MedicationId == medicationId));
            return associations.FirstOrDefault();
        }
    }
}
