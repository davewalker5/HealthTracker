using HealthTracker.Logic.Extensions;

namespace HealthTracker.Tests.Extensions
{
    [TestClass]
    public class StringCleanerTest
    {
        [TestMethod]
        public void RemoveInvalidCharactersTest()
        {
            var result = StringCleaner.RemoveInvalidCharacters(",\r\n");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void CleanTest()
        {
            var result = StringCleaner.Clean("indoor, RuNNing\r\n");
            Assert.AreEqual("Indoor Running", result);
        }
    }
}
