using HealthTracker.Configuration.Entities;
using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Medications;
using HealthTracker.Logic.Extensions;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthTracker.Tests.MedicationTracking
{
    [TestClass]
    public class MedicationStockUpdaterTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly Medication _medication = DataGenerator.RandomMedication();
        private readonly int DailyDose = DataGenerator.RandomInt(1, 4);
        private IHealthTrackerFactory _factory;
        private int _leadTimeDays = DataGenerator.RandomInt(10, 20);
        private int _associationId;

        [TestInitialize]
        public async Task TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var settings = new HealthTrackerApplicationSettings{ MedicationOrderLeadTimeDays = _leadTimeDays };
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, settings, logger.Object);

            var medication = DataGenerator.RandomMedication();
            medication.Id = (await _factory.Medications.AddAsync(_medication.Name)).Id;

            _person.Id = (await _factory.People.AddAsync(_person.FirstNames, _person.Surname, _person.DateOfBirth, _person.Height, _person.Gender)).Id;
            _associationId = Task.Run(() => _factory.PersonMedications.AddAsync(_person.Id, medication.Id, DailyDose, 0, null)).Result.Id;
        }

        [TestMethod]
        public async Task AddStockTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);

            var expectedStock = 0;
            for (int i = 0; i < 5; i++)
            {
                var numberToAdd = DataGenerator.RandomInt(1, 10);
                expectedStock += numberToAdd;
                await _factory.MedicationStockUpdater.AddStockAsync(_associationId, numberToAdd);
            }

            associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(expectedStock, associations[0].Stock);
        }

        [TestMethod]
        public async Task SetStockTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);

            var expectedStock = DataGenerator.RandomInt(1, 100);
            await _factory.MedicationStockUpdater.SetStockAsync(_associationId, expectedStock);

            associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(expectedStock, associations[0].Stock);
        }

        [TestMethod]
        public async Task DecrementTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            await _factory.MedicationStockUpdater.SetStockAsync(_associationId, doses * DailyDose);

            associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            await _factory.MedicationStockUpdater.DecrementAsync(_associationId, doses);

            associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.AreEqual(HealthTrackerDateExtensions.TodayWithoutTime(), associations[0].LastTaken);
        }

        [TestMethod]
        public async Task DecrementAllTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            await _factory.MedicationStockUpdater.SetStockAsync(_associationId, doses * DailyDose);

            associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            await _factory.MedicationStockUpdater.DecrementAllAsync(_associationId, doses);

            associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.AreEqual(HealthTrackerDateExtensions.TodayWithoutTime(), associations[0].LastTaken);
        }

        [TestMethod]
        public async Task DecrementAllIgnoresInactiveAssociationsTest()
        {
            var doses = DataGenerator.RandomInt(1, 10);
            await _factory.PersonMedications.DeactivateAsync(_associationId);
            await _factory.MedicationStockUpdater.SetStockAsync(_associationId, doses * DailyDose);

            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            await _factory.MedicationStockUpdater.DecrementAllAsync(_associationId, doses);

            associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);
        }

        [TestMethod]
        public async Task IncrementTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            await _factory.MedicationStockUpdater.IncrementAsync(_associationId, doses);

            associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.AreEqual(HealthTrackerDateExtensions.TodayWithoutTime(),associations[0].LastTaken);
        }

        [TestMethod]
        public async Task IncrementAllTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            await _factory.MedicationStockUpdater.IncrementAllAsync(_associationId, doses);

            associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.AreEqual(HealthTrackerDateExtensions.TodayWithoutTime(),associations[0].LastTaken);
        }

        [TestMethod]
        public async Task IncrementAllIgnoresInactiveAssociationsTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            await _factory.PersonMedications.DeactivateAsync(_associationId);
            await _factory.MedicationStockUpdater.IncrementAllAsync(_associationId, doses);

            associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(0, associations[0].Stock);
            Assert.IsNull(associations[0].LastTaken);
        }

        [TestMethod]
        public async Task FastForwardTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            var association = await context.PersonMedications.FirstAsync(x => x.Id == _associationId);

            Assert.AreEqual(0, association.Stock);
            Assert.IsNull(association.LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            association.Stock = doses * DailyDose;
            var today = HealthTrackerDateExtensions.TodayWithoutTime();
            association.LastTaken = today.AddDays(-doses + 1);
            await context.SaveChangesAsync();

            await _factory.MedicationStockUpdater.FastForwardAsync(_associationId);

            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(DailyDose, associations[0].Stock);
            Assert.AreEqual(today, associations[0].LastTaken);
        }

        [TestMethod]
        public async Task FastForwardAllTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            var association = await context.PersonMedications.FirstAsync(x => x.PersonId == _person.Id);

            Assert.AreEqual(0, association.Stock);
            Assert.IsNull(association.LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            association.Stock = doses * DailyDose;
            var today = HealthTrackerDateExtensions.TodayWithoutTime();
            association.LastTaken = today.AddDays(-doses + 1);
            await context.SaveChangesAsync();

            await _factory.MedicationStockUpdater.FastForwardAllAsync(_person.Id);

            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(DailyDose, associations[0].Stock);
            Assert.AreEqual(today, associations[0].LastTaken);
        }

        [TestMethod]
        public async Task FastForwardAllIgnoresInactiveAssociationsTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            var association = await context.PersonMedications.FirstAsync(x => x.PersonId == _person.Id);

            Assert.AreEqual(0, association.Stock);
            Assert.IsNull(association.LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            association.Stock = doses * DailyDose;
            await context.SaveChangesAsync();

            await _factory.PersonMedications.DeactivateAsync(_associationId);
            await _factory.MedicationStockUpdater.FastForwardAllAsync(_person.Id);

            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, association.Stock);
            Assert.IsNull(association.LastTaken);
        }

        [TestMethod]
        public async Task SkipTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            var association = await context.PersonMedications.FirstAsync(x => x.Id == _associationId);

            Assert.AreEqual(0, association.Stock);
            Assert.IsNull(association.LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            association.Stock = doses * DailyDose;
            var today = HealthTrackerDateExtensions.TodayWithoutTime();
            association.LastTaken = today.AddDays(-1);
            await context.SaveChangesAsync();

            await _factory.MedicationStockUpdater.SkipAsync(_associationId);

            var associations = await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.AreEqual(today, associations[0].LastTaken);
        }

        [TestMethod]
        public async Task SkipAllTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            var association = await context.PersonMedications.FirstAsync(x => x.PersonId == _person.Id);

            Assert.AreEqual(0, association.Stock);
            Assert.IsNull(association.LastTaken);

            var doses = DataGenerator.RandomInt(1, 10);
            association.Stock = doses * DailyDose;
            var today = HealthTrackerDateExtensions.TodayWithoutTime();
            association.LastTaken = today.AddDays(-1);
            await context.SaveChangesAsync();

            await _factory.MedicationStockUpdater.SkipAllAsync(_person.Id);

            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.AreEqual(today, associations[0].LastTaken);
        }


        [TestMethod]
        public async Task SkipAllIgnoresInactiveAssociationsTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            var association = await context.PersonMedications.FirstAsync(x => x.PersonId == _person.Id);

            var doses = DataGenerator.RandomInt(1, 10);
            association.Stock = doses * DailyDose;
            await context.SaveChangesAsync();

            await _factory.PersonMedications.DeactivateAsync(association.Id);
            await _factory.MedicationStockUpdater.SkipAllAsync(_person.Id);

            var associations = await _factory.PersonMedications.ListAsync(x => x.PersonId == _person.Id, 1, int.MaxValue);
            Assert.AreEqual(doses * DailyDose, associations[0].Stock);
            Assert.IsNull(association.LastTaken);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidStockLevelException))]
        public async Task CannotAddInvalidStockTest()
            => await _factory.MedicationStockUpdater.AddStockAsync(_associationId, -23);

        [TestMethod]
        [ExpectedException(typeof(InvalidStockLevelException))]
        public async Task CannotSetInvalidStockTest()
            => await _factory.MedicationStockUpdater.SetStockAsync(_associationId, -7);
    }
}