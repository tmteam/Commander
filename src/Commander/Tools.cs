using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
namespace Commander
{
    public static class Tools
    {
        public static Type GetNonNullType(Type type)
        {
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                     return type.GenericTypeArguments.First();
                else
                return type;
        }
        public static object Convert(string value, Type type)
        {
            //check for nullable types: (https://msdn.microsoft.com/en-us/library/ms366789.aspx)
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                if (string.IsNullOrWhiteSpace(value))
                    return null;
                else
                     type = type.GenericTypeArguments.First();
            }
            if (type == typeof(bool))
            {
                if (value == "1" || value.ToLower() == "true" || value == "yes")
                    return true;
                if (value == "0" || value.ToLower() == "false" || value == "no")
                    return false;
                throw new InvalidCastException("bool value should be equal one of these strings: 1, 0, true, yes, false, no");
            }
            if (type == typeof(string))
                return value;
            if (type == typeof(DateTime))
                return new DateTimeValue(value).ToDateTime();
            if (type == typeof(TimeSpan))
                return new DateTimeValue(value).ToTimeSpan();
            else
                return System.Convert.ChangeType(value, type);
        }

        public static string DescribeResults(object value, string linePrefix = null) {
            StringBuilder builder;
            if (value == null)
                return "{no results}";
            if (value is string)
                return value.ToString();
        
            var source = value as IEnumerable;
            if (source != null) {
                builder = new StringBuilder("[" + source.Cast<object>().Count<object>() + "]\r\n");
                foreach (object obj2 in source) 
                    builder.Append("\r\n" + DescribeResults(obj2, linePrefix));
                return builder.ToString();
            }

            var array = value as Array;
            if (array != null) {
                builder = new StringBuilder("[" + array.Length + "]\r\n");
                foreach (object obj2 in source)
                    builder.AppendLine("\r\n" + DescribeResults(obj2, linePrefix));
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
                ans.AppendLine(linePrefix + type.Name + ": " + DescribeResults(type.GetValue(value), new string(' ', (linePrefix ?? "").Length + 2)));
            return ans.ToString();
        }

        public static ArgumentDescription[] GetArgumentsDescription(Type type) 
        {
            return type.GetProperties().Select(p=>
                 new ArgumentDescription { 
                    Property = p,
                    Description 
                        = (IArgumentDescription)p.GetCustomAttribute<CommandArgumentAttribute>() 
                        ??(IArgumentDescription)p.GetCustomAttribute<FlagArgumentAttribute>()
                }).Where(c=> c.Description != null).ToArray();
        }

        public static CommandAttribute GetCommandAttributeOrThrow(Type type)
        {
            var customAttribute = type.GetCustomAttribute<CommandAttribute>(false);
            if (customAttribute == null)
                throw new ArgumentException("Got no command attribute");
            return customAttribute;
        }

        public static string NormilizeCommandTypeName(string name)
        {
            if (name.EndsWith("Command"))
                name = name.Remove(name.Length - 7);
            return name;
        }
        static DateTimeValue ParseDateTimeValue(string inputString)
        {
            DateTimeValue ans = new DateTimeValue();
            inputString = inputString.Trim();
            
            return ans;
        }
        public static List<string> SmartSplit(string inputString)
        {
            //("([^\\"]*(\\")*)*"|-(\s*)\S|\S)+
            var pattern = "(\"([^\\\\\"]*(\\\\\")*)*\"|-(\\s*)\\S|\\S)+";
            var regex = new Regex(pattern);
            return regex.Matches(inputString)
                .Cast<Match>()
                .Select(c=>c.Value.Replace("\\",""))//it's kind of a dirty hack. The quest: "put it inside the regexp".
                .ToList();
        }
        public static string[] ParseToConsoleArgs(string inputString)
        {
            string pattern = "(\"([^\\\"]*(\\\")*)*\"|-(\\s*)\\S|\\S)+";
            Regex regex = new Regex(pattern);
            return (from c in regex.Matches(inputString).Cast<Match>() select c.Value).ToArray<string>();
        }
        public static string ExtractCommandName(List<string> args)
        {
            if (args.Count == 0)
                throw new ArgumentException();
            var ans = args[0];
            args.RemoveAt(0);
            return ans;
        }
        public static void ExtractAnsSetToProperties(List<string> args, IEnumerable<ArgumentDescription> properties, object target){
            List<string> notFoundedProperties = new List<string>();
            foreach (var description in properties)
            {
                if (!SearchExactAndSetProperty(args, description, properties, target))
                    notFoundedProperties.Add(description.Description.ShortAlias);
            }
            if (notFoundedProperties.Count != 0)
                throw new MissedArgumentsException(notFoundedProperties.ToArray());
            if (args.Count > 0)
                throw new UnknownArgumentsException(args.ToString());
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
                if (args[i].ToLower() == propertyName)
                {
                    //match!
                    var type = GetNonNullType(property.Property.GetType());
                    var hasMore = (i < args.Count - 1);
                    if (type == typeof(bool))//It can be flag!
                    {
                        if (!hasMore) {
                            property.Property.SetValue(target, true);
                            args.RemoveAt(i);
                            return true;
                        } else if (otherProperties.Any(p => p.Description.ShortAlias.ToLower() == args[i + 1])) {
                            property.Property.SetValue(target, true); //it means flag value
                            args.RemoveAt(i);
                            return true;
                        } else {
                            bool value = (bool)Convert(args[i + 1], type);
                            property.Property.SetValue(target, value);
                            args.RemoveAt(i); //extract the name and the value from the list
                            args.RemoveAt(i + 1);
                            return true;
                        }
                    }
                    else
                    {
                        if (!hasMore)
                            throw new InvalidArgumentException(type, "", property.Description.ShortAlias);
                        var nonBoolValue = Convert(args[i + 1], type);
                        property.Property.SetValue(target, nonBoolValue);
                        args.RemoveAt(i); //extract the name and the value from the list
                        args.RemoveAt(i + 1);
                        return true;
                    }
                }
            }
            return property.Description.Optional;
        }
        public static void SetValuesToObject(Dictionary<string, string> args, IEnumerable<ArgumentDescription> properties, object target)
        {
            List<string> list = new List<string>();
            foreach (var description in properties.Where(a=>!a.Description.Optional))
            {
                if (!args.ContainsKey(description.Description.ShortAlias.ToLower()) 
                 && !args.ContainsKey(description.Property.Name.ToLower()))
                    list.Add(description.Property.Name);
            }

            if (list.Count > 0)
                throw new MissedArgumentsException(list.ToArray());
       
            foreach (KeyValuePair<string, string> pair in args) {
                var name = pair.Key.ToLower();
                var description2 = properties
                    .FirstOrDefault<ArgumentDescription>(a => (a.Description.ShortAlias.ToLower() == name) || (a.Property.Name.ToLower() == name));
                if (description2 == null)
                    throw new UnknownArgumentsException(pair.Key);
                try {
                    var obj2 = Convert(pair.Value, description2.Property.PropertyType);
                    description2.Property.SetValue(target, obj2);
                } catch {
                    throw new InvalidArgumentException(description2.Property.PropertyType, pair.Value, name);
                }
            }
        }

        public static bool TryParse(string[] args, out string commandName, out Dictionary<string, string> arguments)
        {
            commandName = "";
            arguments = new Dictionary<string, string>();
            if (args.Length == 0)
                return false;
            commandName = args[0].ToLower();
            if (args.Length != 1) {
                for (int i = 1; i < args.Length; i++) {
                    if (!args[i].StartsWith("-"))
                        return false;
                    var key = args[i].Substring(1).Replace(" ", null).ToLower();
                    if ((args.Length > (i + 1)) && !args[i + 1].StartsWith("-")) {
                        var str2 = args[i + 1].Trim();
                        if ((str2.StartsWith("\"") && str2.EndsWith("\"")) && (str2.Length > 1))
                            str2 = str2.Substring(1, str2.Length - 2).Replace("\\\"", "\"");
                        arguments.Add(key, str2);
                        i++;
                    } else {
                        arguments.Add(key, true.ToString());
                    }
                }
            }
            return true;
        }
    }

    public class DateTimeValue
    {
        public DateTimeValue() { }
        public DateTimeValue(DateTime time) {
            Parse(time);
        }
        public DateTimeValue(TimeSpan timeSpan) {
            Parse(timeSpan);
        }
        public DateTimeValue(string str)
        {
            DateTime time;
            str = str.Trim();
            if (TryParseDateTime(str))
                return;
            else if (TryParseTimeSpan(str))
                return;
            else if (DateTime.TryParse(str, out time)) {
                Parse(time);
                return;
            }
            else
                throw new InvalidCastException();
        }
        bool TryParseTimeSpan(string str)
        {
            char ch = str.ToLower().Last<char>();
            Func<double, TimeSpan> func = null;
            TimeSpan? res = null;
            switch (ch)
            {
                case 'm':
                    func = new Func<double, TimeSpan>(TimeSpan.FromMinutes);
                    break;
                case 's':
                    func = new Func<double, TimeSpan>(TimeSpan.FromSeconds);
                    break;
                case 'w':
                    func = d => TimeSpan.FromDays(d * 7.0);
                    break;
                case 'd':
                    func = new Func<double, TimeSpan>(TimeSpan.FromDays);
                    break;
                case 'h':
                    func = new Func<double, TimeSpan>(TimeSpan.FromHours);
                    break;
                default:
                    {
                        double arg;
                        if (!double.TryParse(str, out arg))
                            return false;
                        else
                            res = TimeSpan.FromMilliseconds(double.Parse(str));
                        break;
                    }
            }
            if (!res.HasValue) {
                double arg2;
                if (!double.TryParse(str.Remove(str.Length - 1), out arg2))
                    return false;
                res = func(arg2);
            }
            Parse(res.Value);
            return true;
        }
        void Parse(TimeSpan timeSpan)
        {
            Year = null;
            Month = null;
            Day = timeSpan.Days;
            Hour = timeSpan.Hours;
            Minute = timeSpan.Minutes;
            Second = timeSpan.Seconds;
        }
        void Parse(DateTime dateTime){
            Year    = dateTime.Year;
            Month   = dateTime.Month;
            Day     = dateTime.Day;
            Hour    = dateTime.Hour;
            Minute  = dateTime.Minute;
            Second  = dateTime.Second;
        }
       
        bool TryParseDateTime(string str)
        {
            var strArray = str.Split(new char[] { ' ' });
            if ((strArray.Length != 1) && (strArray.Length != 2))
                throw new InvalidCastException();

            string dateStr = null;
            string timeStr = null;

            if (strArray[0].Contains<char>('.'))
                dateStr = strArray[0];
            else if (strArray[0].Contains<char>(':'))
                timeStr = strArray[0];
            if (strArray.Length > 1)
            {
                if (timeStr != null)
                    return false;
                if (!strArray[1].Contains<char>(':'))
                    return false;
                timeStr = strArray[1];
            }
            if (string.IsNullOrWhiteSpace(dateStr) && string.IsNullOrWhiteSpace(timeStr))
                return false;
            if (!string.IsNullOrWhiteSpace(dateStr))
            {
                var strArray2 = dateStr.Split(new char[] { '.' });
                if ((strArray2.Length != 2) && (strArray2.Length != 3))
                    return false;
                Day = new int?(int.Parse(strArray2[0]));
                Month = new int?(int.Parse(strArray2[1]));
                if (strArray2.Length == 3)
                    Year = new int?(int.Parse(strArray2[2]));
            }
            if (!string.IsNullOrWhiteSpace(timeStr))
            {
                string[] strArray3 = timeStr.Split(new char[] { ':' });
                if ((strArray3.Length != 2) && (strArray3.Length != 3))
                    return false;
                Hour = new int?(int.Parse(strArray3[0]));
                Minute = new int?(int.Parse(strArray3[1]));
                if (strArray3.Length == 3)
                    Second = new int?(int.Parse(strArray3[2]));
            }
            return true;
        }
        public int? Year;
        public int? Month;
        public int? Day;
        public int? Hour;
        public int? Minute;
        public int? Second;
        public TimeSpan ToTimeSpan()
        {
            var inSeconds = (Second.GetValueOrDefault() 
                + Minute.GetValueOrDefault() * 60 
                + Hour.GetValueOrDefault() * 3600 
                + Day.GetValueOrDefault() * 3600 * 24);
            return TimeSpan.FromSeconds(inSeconds);
        }
        public DateTime ToDateTime()
        {
            return new DateTime(
                Year.HasValue   ? Year.GetValueOrDefault() : DateTime.Now.Year,
                Month.HasValue  ? Month.GetValueOrDefault() : DateTime.Now.Month,
                Day.HasValue    ? Day.GetValueOrDefault() : DateTime.Now.Day,
                Hour.HasValue   ? Hour.GetValueOrDefault() : 0,
                Minute.HasValue ? Minute.GetValueOrDefault() : 0,
                Second.HasValue ? Second.GetValueOrDefault() : 0);
        }
    }

}
