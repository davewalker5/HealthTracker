using HealthTracker.Data;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests
{
    [TestClass]
    public class UserManagerTest
    {
        private readonly string _userName = DataGenerator.RandomUsername();
        private readonly string _password = DataGenerator.RandomPassword();
        private readonly string _updatedPassword = DataGenerator.RandomPassword();
        private IUserManager _userManager;
        private int _userId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _userManager = new HealthTrackerFactory(context, null, logger.Object).Users;
            _userId = Task.Run(() => _userManager.AddAsync(_userName, _password)).Result.Id;
        }

        [TestMethod]
        public void AddAndGetUserTest()
        {
            var user = Task.Run(() => _userManager!.GetAsync(x => x.Id == _userId)).Result;

            Assert.IsNotNull(user);
            Assert.AreEqual(_userName, user.UserName);
            Assert.AreNotEqual(_password, user.Password);
        }

        [TestMethod]
        public void AddDuplicateUserTest()
        {
            Task.Run(() => _userManager!.AddAsync(_userName, _password)).Wait();
            var users = Task.Run(() => _userManager!.ListAsync(x => true)).Result;

            Assert.IsNotNull(users);
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(_userName, users.First().UserName);
            Assert.AreNotEqual(_password, users.First().Password);
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            Task.Run(() => _userManager!.DeleteAsync(_userName)).Wait();
            var users = Task.Run(() => _userManager!.ListAsync(x => true)).Result;

            Assert.IsFalse(users.Any());
        }

        [TestMethod]
        public void GetMissingUserTest()
        {
            var user = Task.Run(() => _userManager!.GetAsync(x => x.Id == -1)).Result;
            Assert.IsNull(user);
        }

        [TestMethod]
        public void ListAllUsersTest()
        {
            var users = Task.Run(() => _userManager!.ListAsync(x => true)).Result;

            Assert.IsNotNull(users);
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(_userName, users.First().UserName);
            Assert.AreNotEqual(_password, users.First().Password);
        }

        [TestMethod]
        public void AuthenticateTest()
        {
            bool authenticated = Task.Run(() => _userManager!.AuthenticateAsync(_userName, _password)).Result;
            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public void FailedAuthenticationTest()
        {
            var password = $"{DataGenerator.RandomPassword()} - not the same as the real password!";
            bool authenticated = Task.Run(() => _userManager!.AuthenticateAsync(_userName, password)).Result;
            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public void SetPassswordTest()
        {
            Task.Run(() => _userManager!.SetPasswordAsync(_userName, _updatedPassword)).Wait();
            bool authenticated = Task.Run(() => _userManager!.AuthenticateAsync(_userName, _updatedPassword)).Result;
            Assert.IsTrue(authenticated);
        }
    }
}
