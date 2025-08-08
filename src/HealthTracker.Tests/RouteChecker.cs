using System.Globalization;
using System.Web;

namespace HealthTracker.Tests
{
    public static class RouteChecker
    {
        private const string DateFormat = "yyyy-MM-dd H:mm:ss";

        /// <summary>
        /// Given two routes that contain encoded dates, confirm the routes are the same allowing for a margin of error in the
        /// dates due to the time taken to start the test
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="dateIndices"></param>
        /// <param name="margin"></param>
        public static void ConfirmDateBasedRoutesAreEqual(string expected, string actual, int[] dateIndices, double margin = 30.0)
        {
            Console.WriteLine("RouteChecker.ConfirmDateBasedRoutesAreEqual():");
            Console.WriteLine($"\tExpected Route = {expected}");
            Console.WriteLine($"\tActual Route = {actual}");

            // Split the two routes into segments and make sure they have equal numbers of segments
            var expectedSegments = expected.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var actualSegments = expected.Split("/", StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(expectedSegments.Length, actualSegments.Length);

            // Iterate over the segments
            for (int i = 0; i < expectedSegments.Length; i++)
            {
                Console.WriteLine($"\tExpected route, segment {i} = {expectedSegments[i]}");
                Console.WriteLine($"\tActual route, segment {i} = {actualSegments[i]}");

                if (!dateIndices.Contains(i))
                {
                    // For segments that don't represent dates, it's just a straightforward string comparison
                    Assert.AreEqual(expectedSegments[i], actualSegments[i]);
                }
                else
                {
                    // Decode the two route segments
                    var expectedDateString = HttpUtility.UrlDecode(expectedSegments[i]);
                    var actualDateString = HttpUtility.UrlDecode(actualSegments[i]);
                    Console.WriteLine($"\tExpected Route, decoded segment {i} = {expectedDateString}");
                    Console.WriteLine($"\tActual Route, decoded segment {i} = {actualDateString}");

                    // Parse them as dates
                    var expectedDate = DateTime.ParseExact(expectedDateString, DateFormat, CultureInfo.InvariantCulture);
                    var actualDate = DateTime.ParseExact(actualDateString, DateFormat, CultureInfo.InvariantCulture);

                    // For date based segments, it's possible the values may vary slightly on account of the time
                    // taken to start the method under test so we need to have a margin of error
                    var difference = Math.Abs((actualDate - expectedDate).TotalSeconds);
                    Console.WriteLine($"\tTime difference = {difference}");
                    Assert.IsTrue(difference <= margin);
                }
            }
        }
    }
}