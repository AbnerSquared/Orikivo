using System;
using System.Reflection;

namespace Orikivo
{
    /// <summary>
    /// Represents a utility class for managing and editing class properties dynamically.
    /// </summary>
    public class ClassEditor
    {
        public static PropertyInfo[] GetProperties<TClass>()
            => typeof(TClass).GetProperties();

        public static Type GetPropertyType<TClass>(string propertyName)
        {
            foreach (PropertyInfo property in GetProperties<TClass>())
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                if (property.Name.ToLower() == propertyName.ToLower())
                    return property.PropertyType;
            }

            return null;
        }

        public static Type GetPropertyType<TClass>(TClass @class, string propertyName)
            where TClass : class
            => GetPropertyType<TClass>(propertyName);

        public static object GetPropertyValue<TClass>(TClass @class, string propertyName)
            where TClass : class
        {
            foreach (PropertyInfo property in GetProperties<TClass>())
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                // TODO: Implement AliasesAttribute for properties
                if (property.Name.ToLower() == propertyName.ToLower())
                    return property.GetValue(@class, null);
            }

            return null;
        }

        public static void SetPropertyValue<TClass>(TClass @class, string propertyName, object value)
            where TClass : class
        {
            foreach (PropertyInfo property in GetProperties<TClass>())
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                if (property.Name.ToLower() == propertyName.ToLower())
                    if (property.PropertyType == value.GetType())
                    {
                        property.SetValue(@class, value);
                        return;
                    }
            }
        }
    }
}
