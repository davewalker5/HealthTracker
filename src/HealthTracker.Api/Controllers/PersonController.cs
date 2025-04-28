using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class PersonController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public PersonController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return person details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Person>> GetPersonByIdAsync(int id)
        {
            var person = await _factory.People.GetAsync(x => x.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        /// <summary>
        /// Return a list of all people in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Person>>> ListAllPeopleAsync()
        {
            var people = await _factory.People.ListAsync(x => true);

            if (people == null)
            {
                return NoContent();
            }

            return people;
        }

        /// <summary>
        /// Add a person from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Person>> AddPersonAsync([FromBody] Person template)
        {
            var person = await _factory.People.AddAsync(
                template.FirstNames,
                template.Surname,
                template.DateOfBirth,
                template.Height,
                template.Gender
            );

            return person;
        }

        /// <summary>
        /// Update a person from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Person>> UpdatePersonAsync([FromBody] Person template)
        {
            var person = await _factory.People.UpdateAsync(
                template.Id,
                template.FirstNames,
                template.Surname,
                template.DateOfBirth,
                template.Height,
                template.Gender
            );

            return person;
        }

        /// <summary>
        /// Delete a person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            // Check the person exists, first
            var person = await _factory.People.GetAsync(x => x.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.People.DeleteAsync(id);
            return Ok();
        }
    }
}
