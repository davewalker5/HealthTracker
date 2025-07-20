using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Collections;

namespace HealthTracker.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class HealthTrackerEntityBase
    {
        /// <summary>
        /// Return the properties of the entity
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder builder = new();

            // Use reflection to iterate over all the properties of the type
            foreach (var propertyInfo in GetType().GetProperties())
            {
                // Get the value, substituting an indicator for NULL
                var value = propertyInfo.GetValue(this);
                if (value == null)
                {
                    value = "null";
                }

                // Separate from previous properties with a ","
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                // Add this property name
                builder.Append(propertyInfo.Name);
                builder.Append(" = ");

                // Handling depends on the property type
                if (value is string)
                {
                    AppendSimpleValue(builder, value);
                }
                else if (value is IEnumerable enumerable)
                {
                    AppendEnumerable(builder, enumerable);
                }
                else if (value is HealthTrackerEntityBase)
                {
                    AppendEntity(builder, value);
                }
                else
                {
                    AppendSimpleValue(builder, value);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Append a simple value to a string builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        private void AppendSimpleValue(StringBuilder builder, object value)
            => builder.Append(value);

        /// <summary>
        /// Append a complex type to a string builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entity"></param>
        private void AppendEntity(StringBuilder builder, object entity)
        {
            builder.Append("[");
            builder.Append(entity);
            builder.Append("]");
        }

        /// <summary>
        /// Append the contents of an enumerable collection to a string builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="enumerable"></param>
        private void AppendEnumerable(StringBuilder builder, IEnumerable enumerable)
        {
            var isFirstItem = true;

            builder.Append("[");
            foreach (var item in enumerable)
            {
                if (!isFirstItem)
                {
                    builder.Append(", ");
                }

                builder.Append(item);
                isFirstItem = false;
            }
            builder.Append("]");
        }
    }
}