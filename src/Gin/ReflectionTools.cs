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
                     Description
                         = (IArgumentDescription)p.GetCustomAttribute<CommandArgumentAttribute>()
                         ?? (IArgumentDescription)p.GetCustomAttribute<FlagArgumentAttribute>()
                 }).Where(c => c.Description != null).ToArray();
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
        public static void ExtractAnsSetToProperties(List<string> args, IEnumerable<ArgumentDescription> properties, object target)
        {
            var notFoundedProperties = new List<string>();
            foreach (var description in properties)
            {
                if (!SearchExactAndSetProperty(args, description, properties, target))
                    notFoundedProperties.Add(description.Description.ShortAlias);
            }
            if (notFoundedProperties.Count != 0)
                throw new MissedArgumentsException(notFoundedProperties.ToArray());
            
        }
        /// <summary>
        /// Searching for the property value in the args list
        /// </summary>
        /// <returns>true, if value have founded or it is optional, false otherwise</returns>
        public static bool SearchExactAndSetProperty(List<string> args, ArgumentDescription property, IEnumerable<ArgumentDescription> otherProperties, object target)
        {
            var propertyName = property.Description.ShortAlias.ToLower();
            for (int i = 0; i < args.Count; i++)
            {
                if (propertyName == ParseTools.NormalizeCommandArgName(args[i])) {
                    //match!
                    var type = ReflectionTools.GetNonNullType(property.Property.PropertyType);
                    var hasMore = (i < args.Count - 1);
                    if (type == typeof(bool))//It can be flag!
                    {
                        if (!hasMore)
                        {
                            property.Property.SetValue(target, true);
                            args.RemoveAt(i);
                            return true;
                        }
                        else if (otherProperties.Any(p => p.Description.ShortAlias.ToLower() == args[i + 1]))
                        {
                            property.Property.SetValue(target, true); //it means flag value
                            args.RemoveAt(i);
                            return true;
                        }
                        else
                        {
                            bool value = (bool)ParseTools.Convert(args[i + 1], type, property.Description.ShortAlias);
                            property.Property.SetValue(target, value);
                            args.RemoveAt(i); //extract the name and the value from the list
                            args.RemoveAt(i);
                            return true;
                        }
                    }
                    else
                    {
                        if (!hasMore)
                            throw new InvalidArgumentException(type, "", property.Description.ShortAlias);
                        var nonBoolValue = ParseTools.Convert(args[i + 1], type, property.Description.ShortAlias);
                        property.Property.SetValue(target, nonBoolValue);
                        args.RemoveAt(i); //extract the name and the value from the list
                        args.RemoveAt(i);
                        return true;
                    }
                }
            }
            return property.Description.Optional;
        }

    }
}
