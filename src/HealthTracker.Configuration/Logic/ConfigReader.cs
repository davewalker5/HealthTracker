﻿using Microsoft.Extensions.Configuration;
using HealthTracker.Configuration.Interfaces;

namespace HealthTracker.Configuration.Logic
{
    public abstract class ConfigReader<T> : IConfigReader<T> where T : class
    {
        /// <summary>
        /// Load and return the application settings from the named JSON-format application settings file
        /// </summary>
        /// <returns></returns>
        public virtual T Read(string jsonFileName)
        {
            // Set up the configuration reader
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile(jsonFileName)
                .Build();

            // Read the application settings section
            IConfigurationSection section = configuration.GetSection("ApplicationSettings");
            var settings = section.Get<T>();

            return settings;
        }
    }
}
