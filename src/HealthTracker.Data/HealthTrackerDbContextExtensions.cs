using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HealthTracker.Data
{
    public static class HealthTrackerDbContextExtensions
    {
        public static DbSet<T> GetDbSet<T>(this DbContext context) where T : class
        {
            // Get the properties of the DbContext
            var properties = context.GetType().GetProperties();

            // Find the property that matches the DbSet<T>
            var dbSetProperty = properties
                .FirstOrDefault(p => p.PropertyType == typeof(DbSet<T>));

            if (dbSetProperty != null)
            {
                return (DbSet<T>)dbSetProperty.GetValue(context);
            }

            throw new InvalidOperationException($"DbSet<{typeof(T).Name}> is not a member of the database context");
        }
    }
}