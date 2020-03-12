using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orikivo.Text
{
    // TODO: rework this class mechanic. Utilize it correctly in order to easily create formattable classes.
    public class ClassFormatter
    {
        private static char _keyOpenChar = '{';
        private static char _keyCloseChar = '}';

        public ClassFormatter()
        {
            Map = new Dictionary<string, string>();
        }

        public static ClassFormatter FromClass<T>() where T : class
        {
            ClassFormatter map = new ClassFormatter();
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                FormattingPropertyAttribute mapProperty = property.GetCustomAttribute<FormattingPropertyAttribute>();

                if (mapProperty != null)
                {
                    // property.Name.ToLower() => OriFormat.Jsonify(property.Name)
                    map.AddKey(property.Name, mapProperty.Name ?? property.Name.ToLower());
                }
            }
            return map;
        }

        public static ClassFormatter FromClass<T>(T root) where T : class
            => FromClass<T>();

        private Dictionary<string, string> Map { get; }

        private void AddKey(string propertyName, string key)
        {
            Map.Add(propertyName, key);
        }

        public bool ContainsProperty(string propertyName)
            => this[propertyName] != null;

        public string this[string propertyName]
        {
            get
            {
                if (Map.ContainsKey(propertyName))
                    return $"{_keyOpenChar}{Map[propertyName]}{_keyCloseChar}";
                else
                    return null;
            }
        }

        public static string Format<T>(string frame, T obj) where T : class
        {
            ClassFormatter map = FromClass<T>();
            // get all properties in a class.
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            // check through all properties in the class specified.
            foreach (PropertyInfo property in properties)
            {
                // future task: find a way to replace only the singular replacement key, as opposed to replacing other
                // reference inserted when replacing.

                // use stringbuilder to separate the class into pieces?
                if (!string.IsNullOrWhiteSpace(map[property.Name]) && property.GetValue(obj) != null)
                {
                    // ignore array properties, as they must be manually handled.
                    //if (property.GetValue(obj).GetType().IsArray)
                        //continue;
                    frame = frame.Replace(map[property.Name], property.GetValue(obj).ToString());
                }
            }
            // return the final string in which specified values have been replaced.
            return frame;
        }
    }
}
