using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using CommonLibrary.Utilities.DateHelpers;

namespace CommonLibrary.Tracking
{
    /// <summary>
    /// Represents the result of changes between two entities in JSON format.
    /// </summary>
    public class EntityChangeResult
    {
        public string OriginalDataJson { get; set; }
        public string NewDataJson { get; set; }
    }

    /// <summary>
    /// Options for customizing entity change tracking behavior.
    /// </summary>
    public class EntityChangeOptions
    {
        public string[] IgnoreContains { get; set; } = Array.Empty<string>();
        public string[] NotIgnoreContains { get; set; } = Array.Empty<string>();
        public bool IgnoreRelations { get; set; } = true;
    }

    /// <summary>
    /// Tracks changes between two entities and serializes the differences.
    /// </summary>
    public static class EntityChangeTracker
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include, // Keep default for other types
            ContractResolver = new IgnoreDefaultDateTimeResolver(), // Use your custom resolver
        };
        /// <summary>
        /// Gets the changes between two entities and returns the differences in JSON format.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="originalEntity">The original entity.</param>
        /// <param name="updatedEntity">The updated entity.</param>
        /// <param name="options">Options for customizing the tracking behavior.</param>
        /// <returns>An EntityChangeResult with JSON representations of the changes.</returns>
        public static EntityChangeResult GetChanges<T>(T originalEntity, T updatedEntity, EntityChangeOptions options = default) where T : class, new()
        {
            jsonSettings.ContractResolver = new IgnoreResolver(options);

            // Handle cases where one of the entities is null
            if (originalEntity == null || updatedEntity == null)
            {
                var nonNullEntity = originalEntity ?? updatedEntity;
                var filteredData = new Dictionary<string, object>();
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    object value = property.GetValue(nonNullEntity, null);

                    // Skip null properties
                    if (value == null)
                        continue;

                    string propertyName = property.Name.ToLower();

                    // Apply IgnoreContains and NotIgnoreContains rules
                    if (options.NotIgnoreContains.Any(x => propertyName.Contains(x.ToLower())))
                    {
                        filteredData[property.Name] = value;
                    }
                    else if (!options.IgnoreContains.Any(x => propertyName.Contains(x.ToLower())))
                    {
                        filteredData[property.Name] = value;
                    }
                }

                return new EntityChangeResult
                {
                    OriginalDataJson = originalEntity != null ? JsonConvert.SerializeObject(filteredData, jsonSettings) : "{}",
                    NewDataJson = updatedEntity != null ? JsonConvert.SerializeObject(filteredData, jsonSettings) : "{}"
                };
            }

            var originalData = new Dictionary<string, object>();
            var newData = new Dictionary<string, object>();
            PropertyInfo[] propertiesForComparison = typeof(T).GetProperties();

            foreach (PropertyInfo property in propertiesForComparison)
            {
                object originalValue = property.GetValue(originalEntity, null);
                object updatedValue = property.GetValue(updatedEntity, null);

                // Check if the value has changed
                if (!Equals(originalValue, updatedValue))
                {
                    string propertyName = property.Name.ToLower();

                    // Apply IgnoreContains and NotIgnoreContains rules
                    if (options.NotIgnoreContains.Any(x => propertyName.Contains(x.ToLower())))
                    {
                        originalData[property.Name] = originalValue;
                        newData[property.Name] = updatedValue;
                    }
                    else if (!options.IgnoreContains.Any(x => propertyName.Contains(x.ToLower())))
                    {
                        originalData[property.Name] = originalValue;
                        newData[property.Name] = updatedValue;
                    }
                }
            }

            return new EntityChangeResult
            {
                OriginalDataJson = JsonConvert.SerializeObject(originalData, jsonSettings),
                NewDataJson = JsonConvert.SerializeObject(newData, jsonSettings)
            };
        }



        /// <summary>
        /// Sets the value of a property on the entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to set.</param>
        public static T SetPropertyValue<T>(this T entity, string propertyName, object value)
        {
            PropertyInfo propertyInfo = entity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            try
            {
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(entity, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                }
            }
            catch (Exception)
            {
                // Log or handle the exception if needed
            }

            return entity;
        }

        /// <summary>
        /// Gets the value of a property from an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="anonymousObject">The entity.</param>
        /// <param name="propertyName">The name of the property.</param>
        public static object GetObjectValue<T>(this T anonymousObject, string propertyName)
        {
            try
            {
                if (anonymousObject == null) throw new ArgumentNullException(nameof(anonymousObject));
                if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

                PropertyInfo propertyInfo = anonymousObject.GetType().GetProperty(propertyName);
                if (propertyInfo == null) throw new InvalidOperationException($"Property '{propertyName}' not found on type '{anonymousObject.GetType()}'.");

                return propertyInfo.GetValue(anonymousObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Custom contract resolver to control serialization behavior for entity changes.
        /// </summary>
        public class IgnoreResolver : DefaultContractResolver
        {
            private readonly EntityChangeOptions _options;

            public IgnoreResolver(EntityChangeOptions options = default)
            {
                _options = options;
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                // Ignore collection-type properties except strings
                if (_options.IgnoreRelations && (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string)))
                {
                    property.Ignored = true;
                }

                // Ignore properties based on the IgnoreContains option
                if (_options.IgnoreContains.Any(x => property.PropertyName.ToLower().Contains(x.ToLower())) &&
                    !_options.NotIgnoreContains.Any(x => property.PropertyName.ToLower().Contains(x.ToLower())))
                {
                    property.Ignored = true;
                }

                // Add logic to handle DateTime and Nullable<DateTime>
                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    // Explicitly set DefaultValueHandling to Ignore for DateTime and Nullable<DateTime>
                    property.DefaultValueHandling = DefaultValueHandling.Ignore;

                    // Customize the ShouldSerialize method to ignore DateTime.MinValue and null values
                    property.ShouldSerialize = instance =>
                    {
                        var value = property.ValueProvider.GetValue(instance);

                        // For Nullable<DateTime>, check if the value is null or DateTime.MinValue
                        if (value == null || (value is DateTime dateTime && dateTime == DateTime.MinValue))
                        {
                            return false; // Don't serialize if it's null or DateTime.MinValue
                        }

                        return true; // Otherwise, serialize the property
                    };
                }

                return property;
            }
        }
    }
}
