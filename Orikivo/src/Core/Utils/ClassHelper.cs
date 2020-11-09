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

        public static PropertyInfo[] GetProperties(object @class)
            => @class.GetType().GetProperties();


        public static Type GetPropertyType(object @class, string propertyName)
        {
            foreach (PropertyInfo property in GetProperties(@class))
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

        public static PropertyInfo GetProperty(object @class, string propertyName)
        {
            foreach (PropertyInfo property in GetProperties(@class))
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

        public static PropertyInfo GetProperty<TClass>(string propertyName)
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

        public static object GetPropertyValue(object @class, string propertyName)
        {
            foreach (PropertyInfo property in GetProperties(@class))
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

        public static void SetPropertyValue(object @class, string propertyName, object value)
        {
            foreach (PropertyInfo property in GetProperties(@class))
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                IdAttribute id = property.GetCustomAttribute<IdAttribute>();
                AliasesAttribute aliases = property.GetCustomAttribute<AliasesAttribute>();

                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                    || (aliases?.Aliases.Any(x => x == propertyName.ToLower()) ?? false)
                    || (id?.Id?.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ?? false))
                    if (property.PropertyType == value?.GetType()
                        || (property.PropertyType == typeof(string) && value == null)
                        || (property.PropertyType == typeof(Nullable) && value == null))
                    {
                        property.SetValue(@class, value);
                        return;
                    }
            }
        }
    }
}
