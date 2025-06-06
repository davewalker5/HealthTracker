using Microsoft.EntityFrameworkCore;
using HealthTracker.Entities;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.Data;
using HealthTracker.Logic.Extensions;
using System.Linq.Expressions;
using HealthTracker.Entities.Logging;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Logic.Database
{
    public class PersonManager : DatabaseManagerBase, IPersonManager
    {
        internal PersonManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first person matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Person> GetAsync(Expression<Func<Person, bool>> predicate)
        {
            var people = await ListAsync(predicate, 1, int.MaxValue);
            return people.FirstOrDefault();
        }

        /// <summary>
        /// Return all people matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Person>> ListAsync(Expression<Func<Person, bool>> predicate, int pageNumber, int pageSize)
            => await Context.People
                            .Where(predicate)
                            .OrderBy(x => x.Surname)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add a person
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dob"></param>
        /// <param name="height"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public async Task<Person> AddAsync(string firstnames, string surname, DateTime dob, decimal height, Gender gender)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding person: '{firstnames} {surname}', DoB {dob.ToShortDateString()}, Height {height:.00}");

            var person = new Person
            {
                FirstNames = firstnames,
                Surname = surname,
                DateOfBirth = HealthTrackerDateExtensions.DateWithoutTime(dob),
                Height = height,
                Gender = gender
            };

            await Context.People.AddAsync(person);
            await Context.SaveChangesAsync();

            return person;
        }

        
        /// <summary>
        /// Update the properties of the specified person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dob"></param>
        /// <param name="height"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public async Task<Person> UpdateAsync(int id, string firstnames, string surname, DateTime dob, decimal height, Gender gender)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating person with ID {id}: '{firstnames} {surname}', DoB {dob.ToShortDateString()}, Height {height:.00}");

            var person = Context.People.FirstOrDefault(x => x.Id == id);
            if (person != null)
            {
                // Save the changes
                person.FirstNames = firstnames;
                person.Surname = surname;
                person.DateOfBirth = HealthTrackerDateExtensions.DateWithoutTime(dob);
                person.Height = height;
                person.Gender = gender;
                await Context.SaveChangesAsync();
            }

            return person;
        }

        /// <summary>
        /// Delete the person with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting person with ID {id}");

            // Find the person record to check it exists
            var person = await GetAsync(x => x.Id == id);
            if (person != null)
            {
                // Check for weight, blood pressure and exercise measurements. If there are any, the person can't be
                // deleted
                RaisePersonInUseException<WeightMeasurement>(id, "weight measurements");
                RaisePersonInUseException<BloodPressureMeasurement>(id, "blood pressure measurements");
                RaisePersonInUseException<ExerciseMeasurement>(id, "exercise measurements");

                // Delete the person record and save changes
                Factory.Context.Remove(person);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to delete a person with associated data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="personId"></param>
        /// <param name="measurementType"></param>
        /// <exception cref="PersonInUseException"></exception>
        private void RaisePersonInUseException<T>(int personId, string measurementType) where T : MeasurementBase
        {
            var set = Context.GetDbSet<T>();
            var measurement = set.FirstOrDefault(x => x.PersonId == personId);

            if (measurement != null)
            {
                var message = $"Person with Id {personId} has {measurementType} and cannot be deleted";
                throw new PersonInUseException(message);
            }
        }
    }
}