using System.Globalization;
using System.Text;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using HealthTracker.Entities.Medications;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Tests.Mocks
{
    public static class DataGenerator
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

        private static Random _generator = new();

        /// <summary>
        /// Return a random integer in the specified range
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static int RandomInt(int minimum, int maximum)
            => _generator.Next(minimum, maximum + 1);

        /// <summary>
        /// Return a random decimal in the specified range
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static decimal RandomDecimal(decimal minimum, decimal maximum)
            => Math.Round((decimal)_generator.NextDouble() * (maximum - minimum) + minimum, 2, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Generate a random alphanumeric word of the specified length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomWord(int minimumLength, int maximumLength)
        {
            // Generate a random length for the word, within the specified limits
            var length = minimumLength == maximumLength ? minimumLength : RandomInt(minimumLength, maximumLength);

            // Iterate over the number of characters
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                // Select a random offset within the character set and append that character
                var offset = (int)_generator.NextInt64(0, Letters.Length);
                builder.Append(Letters[offset]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate a phrase consisting of the specified number of words 
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static string RandomPhrase(int words, int minimumWordLength, int maximumWordLength)
        {
            StringBuilder builder = new StringBuilder();

            // Iterate over the required number of words
            for (int i = 0; i < words; i++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                }

                var wordLength = (int)_generator.NextInt64(1, 16);
                var word = RandomWord(minimumWordLength, maximumWordLength);
                builder.Append(word);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate a random phrase in title case
        /// </summary>
        /// <param name="words"></param>
        /// <param name="minimumWordLength"></param>
        /// <param name="maximumWordLength"></param>
        /// <returns></returns>
        public static string RandomTitleCasePhrase(int words, int minimumWordLength, int maximumWordLength)
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(RandomPhrase(words, minimumWordLength, maximumWordLength).ToLower());

        /// <summary>
        /// Generate a random date in the specified year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="includeTime"></param>
        /// <returns></returns>
        public static DateTime RandomDateInYear(int year, bool includeTime = false)
        {
            // Pick a random month and find the number of days in that month in the specified year
            var month = RandomInt(1, 12);
            var daysInMonth = DateTime.DaysInMonth(year, month);

            // If required, generate a random time
            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            if (includeTime)
            {
                hours = RandomInt(0, 23);
                minutes = RandomInt(0, 59);
                seconds = RandomInt(0, 59);
            }

            // Pick a random day in the month and construct the date with time set to 00:00:00
            var day = RandomInt(1, daysInMonth);
            var date = new DateTime(year, month, day, hours, minutes, seconds);
            return date;
        }

        /// <summary>
        /// Return the path to a temporary CSV file
        /// </summary>
        /// <returns></returns>
        public static string TemporaryCsvFilePath()
            => Path.ChangeExtension(Path.GetTempFileName(), "csv");

        /// <summary>
        /// Return the path to a temporary XLSX file
        /// </summary>
        /// <returns></returns>
        public static string TemporaryXlsxFilePath()
            => Path.ChangeExtension(Path.GetTempFileName(), "xlsx");

        /// <summary>
        /// Return the path to a temporary context file
        /// </summary>
        /// <returns></returns>
        public static string TemporaryContextFilePath()
            => Path.ChangeExtension(Path.GetTempFileName(), "dat");

        /// <summary>
        /// Generate a random API token
        /// </summary>
        /// <returns></returns>
        public static string RandomApiToken()
            => RandomWord(128, 128);

        /// <summary>
        /// Generate a random user name
        /// </summary>
        /// <returns></returns>
        public static string RandomUsername()
            => RandomWord(16, 16);

        /// <summary>
        /// Generate a random password
        /// </summary>
        /// <returns></returns>
        public static string RandomPassword()
            => RandomWord(15, 30);

        /// <summary>
        /// Generate a random entity ID
        /// </summary>
        /// <returns></returns>
        public static int RandomId()
            => RandomInt(1, 32767);

        /// <summary>
        /// Generate a random activty type name
        /// </summary>
        /// <returns></returns>
        public static string RandomActivityTypeName()
            => RandomTitleCasePhrase(RandomInt(1, 3), 5, 15);

        /// <summary>
        /// Generate a random activity type
        /// </summary>
        /// <returns></returns>
        public static ActivityType RandomActivityType()
            => new()
            {
                Id = RandomId(),
                Description = RandomActivityTypeName(),
                DistanceBased = RandomInt(0, 100) > 50
            };

        /// <summary>
        /// Generate random first names
        /// </summary>
        /// <returns></returns>
        public static string RandomFirstNames()
            => RandomTitleCasePhrase(2, 7, 12);

        /// <summary>
        /// Generate a random surnamne
        /// </summary>
        /// <returns></returns>
        public static string RandomSurname()
        {
            var word = RandomWord(6, 15);
            return $"{char.ToUpper(word[0])}{word[1..]}";
        }

        /// <summary>
        /// Return a random gender
        /// </summary>
        /// <returns></returns>
        public static Gender RandomGender()
            => (Gender)RandomInt(0, 2);

        /// <summary>
        /// Generate a random height in cm
        /// </summary>
        /// <returns></returns>
        public static decimal RandomHeight()
            => RandomDecimal(1.5M, 2M);

        /// <summary>
        /// Generate a random date of birth
        /// </summary>
        /// <param name="minimumAge"></param>
        /// <param name="maximumAge"></param>
        /// <returns></returns>
        public static DateTime RandomDateOfBirth(int minimumAge, int maximumAge)
        {
            // Calculate the valid birthdate range
            DateTime today = DateTime.Today;
            DateTime maximumBirthDate = today.AddYears(-minimumAge);
            DateTime minimumBirthDate = today.AddYears(-maximumAge);

            // Calculate the number of days between them and pick a random number in that range
            int differenceInDays = (maximumBirthDate - minimumBirthDate).Days;
            int daysToAdd = RandomInt(0, differenceInDays);

            // Add the random days to the minimum birth date to get the random DoB
            var dob = minimumBirthDate.AddDays(daysToAdd);
            return dob;
        }

        /// <summary>
        /// Generate a random person
        /// </summary>
        /// <param name="minimumAge"></param>
        /// <param name="maximumAge"></param>
        /// <returns></returns>
        public static Person RandomPerson(int minimumAge, int maximumAge)
            => new()
            {
                Id = RandomId(),
                FirstNames = RandomFirstNames(),
                Surname = RandomSurname(),
                DateOfBirth = RandomDateOfBirth(minimumAge, maximumAge),
                Height = RandomHeight(),
                Gender = RandomGender()
            };

        /// <summary>
        /// Generate a random % SPO2 value
        /// </summary>
        /// <returns></returns>
        public static decimal RandomSPO2Value()
            => RandomDecimal(90, 100);

        /// <summary>
        /// Generate a random % SPO2 measurement for a given person and year
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static BloodOxygenSaturationMeasurement RandomSPO2Measurement(int personId, int year)
            => new()
            {
                Id = RandomId(),
                PersonId = personId,
                Date = RandomDateInYear(year),
                Percentage = RandomSPO2Value(),
                Assessment = "Normal"
            };

        /// <summary>
        /// Generate a random blood glucose value
        /// </summary>
        /// <returns></returns>
        public static decimal RandomBloodGlucoseValue()
            => RandomDecimal(3, 22);

        /// <summary>
        /// Generate a random blood glucose measurement for a given person and year
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static BloodGlucoseMeasurement RandomBloodGlucoseMeasurement(int personId, int year)
            => new()
            {
                Id = RandomId(),
                PersonId = personId,
                Date = RandomDateInYear(year),
                Level = RandomBloodGlucoseValue()
            };

        /// <summary>
        /// Generate a random blood pressure measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <param name="minimumSystolic"></param>
        /// <param name="maximumSystolic"></param>
        /// <param name="minimumDiastolic"></param>
        /// <param name="maximumDiastolic"></param>
        /// <returns></returns>
        public static BloodPressureMeasurement RandomBloodPressureMeasurement(
            int personId,
            DateTime date,
            int minimumSystolic,
            int maximumSystolic,
            int minimumDiastolic,
            int maximumDiastolic)
            => new()
            {
                Id = RandomId(),
                PersonId = personId,
                Date = date,
                Systolic = RandomInt(minimumSystolic, maximumSystolic),
                Diastolic = RandomInt(minimumDiastolic, maximumDiastolic)
            };

        /// <summary>
        /// Generate a random blood pressure measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <param name="minimumSystolic"></param>
        /// <param name="maximumSystolic"></param>
        /// <param name="minimumDiastolic"></param>
        /// <param name="maximumDiastolic"></param>
        /// <returns></returns>
        public static BloodPressureMeasurement RandomBloodPressureMeasurement(
            int personId,
            int year,
            int minimumSystolic,
            int maximumSystolic,
            int minimumDiastolic,
            int maximumDiastolic)
            => RandomBloodPressureMeasurement(personId, RandomDateInYear(year), minimumSystolic, maximumSystolic, minimumDiastolic, maximumDiastolic);

        /// <summary>
        /// Generate a random blood pressure measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <param name="minimumSystolic"></param>
        /// <param name="maximumSystolic"></param>
        /// <param name="minimumDiastolic"></param>
        /// <param name="maximumDiastolic"></param>
        /// <returns></returns>
        public static BloodPressureMeasurement RandomBloodPressureMeasurement(
            int personId,
            int minimumSystolic,
            int maximumSystolic,
            int minimumDiastolic,
            int maximumDiastolic)
            => RandomBloodPressureMeasurement(personId, DateTime.Now, minimumSystolic, maximumSystolic, minimumDiastolic, maximumDiastolic);

        /// <summary>
        /// Generate a random blood pressure reading in systolic/diastolic format
        /// </summary>
        /// <param name="minimumSystolic"></param>
        /// <param name="maximumSystolic"></param>
        /// <param name="minimumDiastolic"></param>
        /// <param name="maximumDiastolic"></param>
        /// <returns></returns>
        public static string RandomBloodPressureReading(int minimumSystolic, int maximumSystolic, int minimumDiastolic, int maximumDiastolic)
            => $"{RandomInt(minimumSystolic, maximumSystolic)}/{RandomInt(minimumDiastolic, maximumDiastolic)}";

        /// <summary>
        /// Generate a random total lipid level for a cholesterol measurement
        /// </summary>
        /// <returns></returns>
        public static decimal RandomTotalLipids()
            => RandomDecimal(2.6M, 5.2M);

        /// <summary>
        /// Generate a random HDL level for a cholesterol measurement
        /// </summary>
        /// <returns></returns>
        public static decimal RandomHDL()
            => RandomDecimal(1.0M, 1.5M);

        /// <summary>
        /// Generate a random LDL level for a cholesterol measurement
        /// </summary>
        /// <returns></returns>
        public static decimal RandomLDL()
            => RandomDecimal(2.6M, 3.3M);

        /// <summary>
        /// Generate a random triglyceride level for a cholesterol measurement
        /// </summary>
        /// <returns></returns>
        public static decimal RandomTriglycerides()
            => RandomDecimal(0.6M, 1.7M);

        /// <summary>
        /// Generate a random cholesterol measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static CholesterolMeasurement RandomCholesterolMeasurement(int personId, int year)
            => new()
            {
                Id = RandomId(),
                PersonId = personId,
                Date = RandomDateInYear(year),
                Total = RandomTotalLipids(),
                HDL = RandomHDL(),
                LDL = RandomLDL(),
                Triglycerides = RandomTriglycerides()
            };

        /// <summary>
        /// Generate a random medication name
        /// </summary>
        /// <returns></returns>
        public static string RandomMedicationName()
            => RandomTitleCasePhrase(RandomInt(1, 2), 5, 15);

        /// <summary>
        /// Generate a random medication
        /// </summary>
        /// <returns></returns>
        public static Medication RandomMedication()
            => new()
            {
                Id = RandomId(),
                Name = RandomMedicationName()
            };

        /// <summary>
        /// Generate a random daily dose of a medication
        /// </summary>
        /// <returns></returns>
        public static int RandomDose()
            => RandomInt(1, 4);

        /// <summary>
        /// Generate a random daily stock of a medication
        /// </summary>
        /// <returns></returns>
        public static int RandomStock()
            => RandomInt(0, 60);

        /// <summary>
        /// Generate a random person/medication association for the specified person and medication and with 
        /// a last taken date of today
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static PersonMedication RandomPersonMedicationAssociation(int personId, int medicationId, bool state)
            => new()
            {
                Id = RandomId(),
                PersonId = personId,
                MedicationId = medicationId,
                DailyDose = RandomDose(),
                Stock = RandomStock(),
                LastTaken = DateTime.Now,
                Active = state
            };

        /// <summary>
        /// Generate a random person/medication association with random IDs and a last taken date of today
        /// </summary>
        /// <returns></returns>
        public static PersonMedication RandomPersonMedicationAssociation()
            => RandomPersonMedicationAssociation(RandomId(), RandomId(), true);

        /// <summary>
        /// Generate a random formatted duration
        /// </summary>
        /// <returns></returns>
        public static string RandomDuration()
            => $"{RandomInt(0, 99):00}:{RandomInt(0, 59):00}:{RandomInt(0, 59):00}";

        /// <summary>
        /// Generate a random exercise measurement for a specified person, activity and year
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static ExerciseMeasurement RandomExerciseMeasurement(int personId, int activityTypeId, DateTime date)
            => new()
            {
                Id = RandomId(),
                PersonId = personId,
                ActivityTypeId = activityTypeId,
                Date = date,
                Duration = RandomInt(1800, 10800),
                Distance = RandomDecimal(0, 50),
                Calories = RandomInt(250, 3200),
                MinimumHeartRate = RandomInt(50, 80),
                MaximumHeartRate = RandomInt(130, 160)
            };

        /// <summary>
        /// Generate a random exercise measurement for the specified year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static ExerciseMeasurement RandomExerciseMeasurement(int year)
            => RandomExerciseMeasurement(RandomId(), RandomId(), RandomDateInYear(year));

        /// <summary>
        /// Generate a random exercise measurement for today
        /// </summary>
        /// <returns></returns>
        public static ExerciseMeasurement RandomExerciseMeasurement()
            => RandomExerciseMeasurement(RandomId(), RandomId(), DateTime.Now);

        /// <summary>
        /// Generate a random weight measurement in a specified BMI band
        /// </summary>
        /// <param name="bands"></param>
        /// <param name="bandName"></param>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static WeightMeasurement RandomWeightMeasurement(IEnumerable<BMIBand> bands, string bandName, int personId, int year, decimal height)
        {
            var band = bands.First(x => x.Name == bandName);
            var bmi = RandomDecimal(band.MinimumBMI, band.MaximumBMI);
            var weight = bmi * (decimal)Math.Pow((double)height, 2);

            return new WeightMeasurement()
            {
                Id = RandomId(),
                PersonId = personId,
                Date = RandomDateInYear(year),
                Weight = weight,
                BMI = bmi,
                BMIAssessment = bandName
            };
        }

        /// <summary>
        /// Generate a random weight measurement within a specified range for a specified individual
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static WeightMeasurement RandomWeightMeasurement(int personId, DateTime date, decimal minimum, decimal maximum)
            => new()
            {
                Id = RandomId(),
                PersonId = personId,
                Date = date,
                Weight = RandomDecimal(minimum, maximum)
            };

        /// <summary>
        /// Generate a random weight measurement within a specified range for a specified individual in the specified year
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="year"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static WeightMeasurement RandomWeightMeasurement(int personId, int year, decimal minimum, decimal maximum)
            => RandomWeightMeasurement(personId, RandomDateInYear(year), minimum, maximum);

        /// <summary>
        /// Generate a random weight measurement within a specified range in a specific year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static WeightMeasurement RandomWeightMeasurement(int year, decimal minimum, decimal maximum)
            => RandomWeightMeasurement(RandomId(), year, minimum, maximum);

        /// <summary>
        /// Generate a random weight measurement within a specified range dated today
        /// </summary>
        /// <param name="year"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static WeightMeasurement RandomWeightMeasurement(decimal minimum, decimal maximum)
            => RandomWeightMeasurement(RandomId(), DateTime.Now, minimum, maximum);

        /// <summary>
        /// Generate a random exercise summary
        /// </summary>
        /// <returns></returns>
        public static ExerciseSummary RandomExerciseSummary()
            => new ExerciseSummary()
            {
                From = RandomDateInYear(2024),
                To = RandomDateInYear(2025),
                Count = RandomInt(100, 1000),
                TotalDuration = RandomInt(1000, 1000000),
                TotalDistance = RandomDecimal(1000, 10000),
                TotalCalories = RandomInt(1000, 100000),
                MinimumHeartRate = RandomInt(50, 80),
                MaximumHeartRate = RandomInt(130, 160)
            };

        /// <summary>
        /// Generate a random blood pressure band
        /// </summary>
        /// <returns></returns>
        public static BloodPressureBand RandomBloodPressureBand()
            => new()
            {
                Id = RandomId(),
                Name = RandomWord(5, 20),
                MinimumSystolic = RandomInt(100, 130),
                MaximumSystolic = RandomInt(130, 200),
                MinimumDiastolic = RandomInt(50, 90),
                MaximumDiastolic = RandomInt(90, 120)
            };

        /// <summary>
        /// Generate a random % SPO2 band
        /// </summary>
        /// <returns></returns>
        public static BloodOxygenSaturationBand RandomBloodOxygenSaturationBand()
            => new()
            {
                Id = RandomId(),
                Name = RandomWord(5, 20),
                MinimumSPO2 = RandomInt(80, 90),
                MaximumSPO2 = RandomInt(90, 100),
                MinimumAge = RandomInt(20, 30),
                MaximumAge = RandomInt(30, 80)
            };

        /// <summary>
        /// Generate a random BMI band
        /// </summary>
        /// <returns></returns>
        public static BMIBand RandomBMIBand()
            => new()
            {
                Id = RandomId(),
                Name = RandomWord(5, 20),
                MinimumBMI = RandomInt(15, 25),
                MaximumBMI = RandomInt(25, 30),
                Order = RandomInt(10, 20)
            };

        /// <summary>
        /// Generate a random job status
        /// </summary>
        /// <returns></returns>
        public static JobStatus RandomJobStatus()
            => new()
            {
                Id = RandomId(),
                Name = RandomWord(10, 20),
                Parameters = $"{RandomWord(10, 20)}.csv",
                Start = RandomDateInYear(2025, true),
                End = RandomDateInYear(2025, true),
                Error = RandomPhrase(5, 5, 10)
            };

        /// <summary>
        /// Generate a random beverage
        /// </summary>
        /// <returns></returns>
        public static Beverage RandomBeverage()
            => new()
            {
                Name = RandomTitleCasePhrase(2, 5, 10),
                TypicalABV = RandomDecimal(0, 40)
            };
    }
}