namespace CommonLibrary.Utilities.DateHelpers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Reflection;

    public class IgnoreDefaultDateTimeResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // Check if the property is of type DateTime or Nullable<DateTime>
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
