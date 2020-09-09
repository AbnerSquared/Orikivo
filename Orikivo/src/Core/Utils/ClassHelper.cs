using System;
using System.Linq;
using System.Reflection;

namespace Orikivo
{
    /// <summary>
    /// Represents a utility class for managing and editing class properties dynamically.
    /// </summary>
    public class ClassHelper
    {
        public static PropertyInfo[] GetProperties<TClass>()
            => typeof(TClass).GetProperties();

        public static Type GetPropertyType<TClass>(string propertyName)
        {
            foreach (PropertyInfo property in GetProperties<TClass>())
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                IdAttribute id = property.GetCustomAttribute<IdAttribute>();
                AliasesAttribute aliases = property.GetCustomAttribute<AliasesAttribute>();

                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                    || (aliases?.Aliases.Any(x => x == propertyName.ToLower()) ?? false)
                    || (id?.Id?.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ?? false))
                    return property.PropertyType;
            }

            return null;
        }

        public static Type GetPropertyType<TClass>(TClass @class, string propertyName)
            where TClass : class
            => GetPropertyType<TClass>(propertyName);

        public static PropertyInfo GetProperty<TClass>(TClass @class, string propertyName)
            where TClass : class
        {
            foreach (PropertyInfo property in GetProperties<TClass>())
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                IdAttribute id = property.GetCustomAttribute<IdAttribute>();
                AliasesAttribute aliases = property.GetCustomAttribute<AliasesAttribute>();

                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                    || (aliases?.Aliases.Any(x => x == propertyName.ToLower()) ?? false)
                    || (id?.Id?.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    return property;
                }
            }

            return null;
        }

        public static object GetPropertyValue<TClass>(TClass @class, string propertyName)
            where TClass : class
        {
            foreach (PropertyInfo property in GetProperties<TClass>())
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                IdAttribute id = property.GetCustomAttribute<IdAttribute>();
                AliasesAttribute aliases = property.GetCustomAttribute<AliasesAttribute>();

                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                    || (aliases?.Aliases.Any(x => x == propertyName.ToLower()) ?? false)
                    || (id?.Id?.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    return property.GetValue(@class, null);
                }
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

                IdAttribute id = property.GetCustomAttribute<IdAttribute>();
                AliasesAttribute aliases = property.GetCustomAttribute<AliasesAttribute>();

                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                    || (aliases?.Aliases.Any(x => x == propertyName.ToLower()) ?? false)
                    || (id?.Id?.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ?? false))
                    if (property.PropertyType == value.GetType())
                    {
                        property.SetValue(@class, value);
                        return;
                    }
            }
        }
    }
}
