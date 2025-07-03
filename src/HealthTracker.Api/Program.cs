using HealthTracker.Api.Interfaces;
using HealthTracker.Api.Services;
using HealthTracker.Data;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Logic.Factory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using HealthTracker.Logic.Logging;
using HealthTracker.Api.Entities;
using HealthTracker.Configuration.Interfaces;
using Microsoft.AspNetCore.Diagnostics;

namespace HealthTracker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Read the configuration file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure strongly typed application settings
            IConfigurationSection section = configuration.GetSection("ApplicationSettings");
            builder.Services.Configure<HealthTrackerApplicationSettings>(section);
            var settings = section.Get<HealthTrackerApplicationSettings>();

            // Configure the DB context
            var connectionString = configuration.GetConnectionString("HealthTrackerDB");
            builder.Services.AddScoped<HealthTrackerDbContext>();
            builder.Services.AddDbContextPool<HealthTrackerDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            // Get the version number and application title
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            var title = $"Health Tracker API v{info.FileVersion}";

            // Create the file logger and log the startup messages
            var logger = new FileLogger();
            logger.Initialise(settings.LogFile, settings.MinimumLogLevel);
            logger.LogMessage(Severity.Info, new string('=', 80));
            logger.LogMessage(Severity.Info, title);

            // Log the connection string
            var message = $"Database connection string = {connectionString}";
            logger.LogMessage(Severity.Info, message);

            // Register the logger with the DI framework
            builder.Services.AddSingleton<IHealthTrackerLogger>(x => logger);

            // Configure the business logic
            builder.Services.AddSingleton<IHealthTrackerApplicationSettings>(settings);
            builder.Services.AddScoped<IHealthTrackerFactory, HealthTrackerFactory>();
            builder.Services.AddScoped<IUserService, UserService>();

            // Add the person exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<PersonExportWorkItem>, BackgroundQueue<PersonExportWorkItem>>();
            builder.Services.AddHostedService<PersonExportService>();

            // Add the person importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<PersonImportWorkItem>, BackgroundQueue<PersonImportWorkItem>>();
            builder.Services.AddHostedService<PersonImportService>();

            // Add the weight measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<WeightMeasurementExportWorkItem>, BackgroundQueue<WeightMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<WeightMeasurementExportService>();

            // Add the weight measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<WeightMeasurementImportWorkItem>, BackgroundQueue<WeightMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<WeightMeasurementImportService>();

            // Add the blood pressure measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BloodPressureMeasurementExportWorkItem>, BackgroundQueue<BloodPressureMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<BloodPressureMeasurementExportService>();

            // Add the daily average blood pressure measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<DailyAverageBloodPressureExportWorkItem>, BackgroundQueue<DailyAverageBloodPressureExportWorkItem>>();
            builder.Services.AddHostedService<DailyAverageBloodPressureExportService>();

            // Add the blood pressure measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BloodPressureMeasurementImportWorkItem>, BackgroundQueue<BloodPressureMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<BloodPressureMeasurementImportService>();

            // Add the OMRON blood pressure measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<OmronBloodPressureImportWorkItem>, BackgroundQueue<OmronBloodPressureImportWorkItem>>();
            builder.Services.AddHostedService<OmronBloodPressureImportService>();

            // Add the % SPO2 measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BloodOxygenSaturationMeasurementExportWorkItem>, BackgroundQueue<BloodOxygenSaturationMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<BloodOxygenSaturationMeasurementExportService>();

            // Add the daily average % SPO2 measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<DailyAverageBloodOxygenSaturationExportWorkItem>, BackgroundQueue<DailyAverageBloodOxygenSaturationExportWorkItem>>();
            builder.Services.AddHostedService<DailyAverageBloodOxygenSaturationExportService>();

            // Add the % SPO2 measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BloodOxygenSaturationMeasurementImportWorkItem>, BackgroundQueue<BloodOxygenSaturationMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<BloodOxygenSaturationMeasurementImportService>();

            // Add the cholesterol measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<CholesterolMeasurementExportWorkItem>, BackgroundQueue<CholesterolMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<CholesterolMeasurementExportService>();

            // Add the cholesterol measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<CholesterolMeasurementImportWorkItem>, BackgroundQueue<CholesterolMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<CholesterolMeasurementImportService>();

            // Add the exercise measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<ExerciseMeasurementExportWorkItem>, BackgroundQueue<ExerciseMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<ExerciseMeasurementExportService>();

            // Add the exercise measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<ExerciseMeasurementImportWorkItem>, BackgroundQueue<ExerciseMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<ExerciseMeasurementImportService>();

            // Add the blood glucose measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BloodGlucoseMeasurementExportWorkItem>, BackgroundQueue<BloodGlucoseMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<BloodGlucoseMeasurementExportService>();

            // Add the blood glucose measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BloodGlucoseMeasurementImportWorkItem>, BackgroundQueue<BloodGlucoseMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<BloodGlucoseMeasurementImportService>();

            // Add the beverage consumption measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BeverageConsumptionMeasurementExportWorkItem>, BackgroundQueue<BeverageConsumptionMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<BeverageConsumptionMeasurementExportService>();

            // Add the beverage consumption measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<BeverageConsumptionMeasurementImportWorkItem>, BackgroundQueue<BeverageConsumptionMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<BeverageConsumptionMeasurementImportService>();

            // Add the food item exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<FoodItemExportWorkItem>, BackgroundQueue<FoodItemExportWorkItem>>();
            builder.Services.AddHostedService<FoodItemExportService>();

            // Add the food item importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<FoodItemImportWorkItem>, BackgroundQueue<FoodItemImportWorkItem>>();
            builder.Services.AddHostedService<FoodItemImportService>();
            
            // Add the meal exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<MealExportWorkItem>, BackgroundQueue<MealExportWorkItem>>();
            builder.Services.AddHostedService<MealExportService>();

            // Add the meal importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<MealImportWorkItem>, BackgroundQueue<MealImportWorkItem>>();
            builder.Services.AddHostedService<MealImportService>();

            // Add the meal consumption measurement exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<MealConsumptionMeasurementExportWorkItem>, BackgroundQueue<MealConsumptionMeasurementExportWorkItem>>();
            builder.Services.AddHostedService<MealConsumptionMeasurementExportService>();

            // Add the meal consumption measurement importer hosted service
            builder.Services.AddSingleton<IBackgroundQueue<MealConsumptionMeasurementImportWorkItem>, BackgroundQueue<MealConsumptionMeasurementImportWorkItem>>();
            builder.Services.AddHostedService<MealConsumptionMeasurementImportService>();

            // Configure JWT
            byte[] key = Encoding.ASCII.GetBytes(settings!.Secret);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var app = builder.Build();

            // Configure the exception handling middleware to write to the log file
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    // Get an instance of the error handling feature
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature?.Error;

                    if (exception != null)
                    {
                        // Log the exception
                        var logger = context.RequestServices.GetRequiredService<IHealthTrackerLogger>();
                        logger.LogMessage(Severity.Error, exception.Message);
                        logger.LogException(exception);

                        // Set a 500 response code and return the exception details in the response
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            Message = exception.Message
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}