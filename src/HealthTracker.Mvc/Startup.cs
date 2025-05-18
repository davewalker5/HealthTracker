using HealthTracker.Mvc.Controllers;
using HealthTracker.Mvc.Api;
using HealthTracker.Client.ApiClient;
using HealthTracker.Configuration.Logic;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Client.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Helpers;

namespace HealthTracker.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Configure strongly typed application settings
            var settings = new HealthTrackerConfigReader().Read("appsettings.json");
            services.AddSingleton<IHealthTrackerApplicationSettings>(provider => settings);

            // The authentication token provider needs to access session via the context
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthenticationTokenProvider, AuthenticationTokenProvider>();

            // Interactions with the REST service are managed via typed HttpClients
            services.AddSingleton<IHealthTrackerHttpClient>(provider => HealthTrackerHttpClient.Instance);
            services.AddSingleton<IAuthenticationClient, AuthenticationClient>();
            services.AddSingleton<IReferenceDataClient, ReferenceDataClient>();
            services.AddSingleton<IActivityTypeClient, ActivityTypeClient>();
            services.AddSingleton<IMedicationClient, MedicationClient>();
            services.AddSingleton<IPersonClient, PersonClient>();
            services.AddSingleton<IBloodGlucoseMeasurementClient, BloodGlucoseMeasurementClient>();
            services.AddSingleton<IBloodOxygenSaturationMeasurementClient, BloodOxygenSaturationMeasurementClient>();
            services.AddSingleton<IBloodPressureMeasurementClient, BloodPressureMeasurementClient>();
            services.AddSingleton<IExerciseMeasurementClient, ExerciseMeasurementClient>();
            services.AddSingleton<IWeightMeasurementClient, WeightMeasurementClient>();

            // Configure the helper used to build the filtering view model used on the measurements pages
            services.AddSingleton<IFilterGenerator, FilterGenerator>();

            // Configure session state for token storage
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Configure JWT
            byte[] key = Encoding.ASCII.GetBytes(settings.Secret);
            services.AddAuthentication(x =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            // JWT authentication with the service is used to authenticate in the UI, so the user data
            // is held in one place (the service database). The login page authenticates with the service
            // and, if successful, stores the JWT token in session. This code segment injects the stored
            // token (if present) into an incoming request
            app.Use(async (context, next) =>
            {
                string token = context.Session.GetString(AuthenticationTokenProvider.TokenSessionKey);
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append("Authorization", "Bearer " + token);
                }
                await next();
            });

            // Await completion of the pipeline. Once it's done, check the status code and, if it's a
            // 401 Unauthorized, redirect to the login page
            app.Use(async (context, previous) =>
            {
                await previous();
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    context.Response.Redirect(LoginController.LoginPath);
                }
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
