using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace TheGin
{
    public static class ReflectionTools
    {
        public static void ThrowIfItIsNotValidCommand(Type commandType)
        {
            if (!typeof(ICommand).IsAssignableFrom(commandType))
            {
                throw new ArgumentException("Does not implement ICommand");
            }
            if (commandType.GetConstructor(new Type[0]) == null)
            {
                throw new ArgumentException("Got no empty constructor");
            }
        }
        public static bool IsNullable(Type type)
        {
            //check for nullable types: (https://msdn.microsoft.com/en-us/library/ms366789.aspx)
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        public static Type GetNonNullType(Type type)
        {
            if (IsNullable(type))
                return type.GenericTypeArguments.First();
            else
                return type;
        }
        public static ArgumentDescription[] GetArgumentsDescription(Type type)
        {
            return type.GetProperties().Select(p =>
                 new ArgumentDescription
                 {
                     Property = p,
                     Attribute = p.GetCustomAttribute<CommandArgumentAttribute>()
                 }).Where(c => c.Attribute != null).ToArray();
        }
        public static CommandAttribute GetCommandAttributeOrThrow(Type type)
        {
            var customAttribute = type.GetCustomAttribute<CommandAttribute>(false);
            if (customAttribute == null)
                throw new ArgumentException("Got no command attribute");
            return customAttribute;
        }
        public static string Describe(object value, string linePrefix = null)
        {
            StringBuilder builder;
            if (value == null)
                return "{no results}";
            if (value is string)
                return value.ToString();

            var source = value as IEnumerable;
            if (source != null)
            {
                builder = new StringBuilder("[" + source.Cast<object>().Count<object>() + "]\r\n");
                foreach (object obj2 in source)
                    builder.Append("\r\n" + Describe(obj2, linePrefix));
                return builder.ToString();
            }

            var array = value as Array;
            if (array != null)
            {
                builder = new StringBuilder("[" + array.Length + "]\r\n");
                foreach (object obj2 in source)
                    builder.AppendLine("\r\n" + Describe(obj2, linePrefix));
                return builder.ToString();
            }

            var typeArray = value
                .GetType()
                .GetProperties()
                .Where(f => f.GetCustomAttribute<ResultAttribute>(true) != null)
                .ToArray();

            if (typeArray.Length == 0)
                return value.ToString();

            var ans = new StringBuilder();
            foreach (var type in typeArray)
                ans.AppendLine(linePrefix + type.Name + ": " + Describe(type.GetValue(value), new string(' ', (linePrefix ?? "").Length + 2)));
            return ans.ToString();
        }
        public static void Configurate(object obj, Dictionary<PropertyInfo, object> _configuration)
        {
            foreach (var property in _configuration)
            {
                property.Key.SetValue(obj, property.Value);
            }

        }
        public static Dictionary<PropertyInfo, object> ExtractAndParse(List<string> args, IEnumerable<ArgumentDescription> properties, Type type)
        {
            var ans = new Dictionary<PropertyInfo, object>();
            var notFoundedProperties = new List<string>();
            foreach (var description in properties)
            {
                var val = SearchExactAndGetValue(args, description, properties, type);

                if (val == null && !description.Attribute.Optional)
                    notFoundedProperties.Add(description.Attribute.ShortAlias);
                else
                    ans.Add(description.Property, val);
            }
            if (notFoundedProperties.Count != 0)
                throw new MissedArgumentsException(notFoundedProperties.ToArray());
            return ans;
        }
        /// <summary>
        /// Searching for the property value in the args list
        /// </summary>
        /// <returns>value, if value have founded, null false otherwise</returns>
        static object SearchExactAndGetValue(List<string> args, ArgumentDescription property, IEnumerable<ArgumentDescription> otherProperties, Type type)
        {
                var propertyName = property.Attribute.ShortAlias.ToLower();
                for (int i = 0; i < args.Count; i++)
                {
                    if (propertyName == ParseTools.NormalizeCommandArgName(args[i]))
                    {
                        //match!
                        var propertyType = ReflectionTools.GetNonNullType(property.Property.PropertyType);
                        var hasMore = (i < args.Count - 1);
                        if (propertyType == typeof(bool))//It can be flag!
                        {
                            if (!hasMore || otherProperties.Any(p => p.Attribute.ShortAlias.ToLower() == args[i + 1])) {
                                args.RemoveAt(i);
                                return true;//it means flag value
                            } else {
                                var value = ParseTools.Convert(args[i + 1], propertyType, property.Attribute.ShortAlias);
                                args.RemoveAt(i); //extract the name and the value from the list
                                args.RemoveAt(i);
                                return value;
                            }
                        }
                        else
                        {
                            if (!hasMore)
                                throw new InvalidArgumentException(type, "", property.Attribute.ShortAlias);
                            var nonBoolValue = ParseTools.Convert(args[i + 1], propertyType, property.Attribute.ShortAlias);
                            args.RemoveAt(i); //extract the name and the value from the list
                            args.RemoveAt(i);
                            return nonBoolValue;
                        }
                    }
                }
                return null;
        }
    }
}
