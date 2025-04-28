using System.Collections.Generic;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class PersonExtensions
    {
        /// <summary>
        /// Return an exportable person from a person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static ExportablePerson ToExportable(this Person person)
            =>  new()
                {
                    FirstNames = person.FirstNames,
                    Surname = person.Surname,
                    DateOfBirth = person.DateOfBirth,
                    Height = person.Height,
                    Gender = person.Gender.ToString()
                };

        /// <summary>
        /// Return a collection of exportable people from a collection of people
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportablePerson> ToExportable(this IEnumerable<Person> people)
        {
            var exportable = new List<ExportablePerson>();

            foreach (var person in people)
            {
                exportable.Add(person.ToExportable());
            }

            return exportable;
        }

        /// <summary>
        /// Return a person from an exportable person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static Person FromExportable(this ExportablePerson person)
            => new()
              {
                  FirstNames = person.FirstNames,
                  Surname = person.Surname,
                  DateOfBirth = person.DateOfBirth,
                  Height = person.Height,
                  Gender = Enum.Parse<Gender>(person.Gender)
            };

        /// <summary>
        /// Return a collection of people from a collection of exportable people
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static IEnumerable<Person> FromExportable(this IEnumerable<ExportablePerson> exportable)
        {
            var people = new List<Person>();

            foreach (var person in exportable)
            {
                people.Add(person.FromExportable());
            }

            return people;
        }
    }
}
