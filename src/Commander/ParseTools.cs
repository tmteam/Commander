using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Commander
{
    public static class ParseTools
    {
        public static readonly string[] trueStrings = new[]{
            "1","true","yes","да","ja","oui"
        };
        public static readonly string[] falseStrings = new[]{
            "1","false","no","нет","nein","non"
        };
        public static object Convert(string value, Type nonNullType, string alias)
        {
            if (ReflectionTools.IsNullable(nonNullType))
                throw new ArgumentException();

            if (value.StartsWith("\"") && value.EndsWith("\""))
                value = value.Remove(value.Length - 2).Substring(1);

            if (nonNullType == typeof(bool))
            {
                value = value.ToLower();
                if (trueStrings.Contains(value))
                    return true;
                if (falseStrings.Contains(value))
                    return false;
            }
            else try
                {
                    if (nonNullType == typeof(string))
                        return value;
                    if (nonNullType == typeof(DateTime))
                        return new DateTimeValue(value).ToDateTime();
                    if (nonNullType == typeof(TimeSpan))
                        return new DateTimeValue(value).ToTimeSpan();
                    else
                        return System.Convert.ChangeType(value, nonNullType);
                }
                catch { }

            throw new InvalidArgumentException(nonNullType, value, alias);
        }
        public static string NormalizeCommandTypeName(string name)
        {
            if (name.EndsWith("Command"))
                name = name.Remove(name.Length - 7);
            return name;
        }
        public static List<string> SmartSplit(string inputString)
        {
            //("([^\\"]*(\\")*)*"|-(\s*)\S|\S)+
            var pattern = "(\"([^\\\\\"]*(\\\\\")*)*\"|-(\\s*)\\S|\\S)+";
            var regex = new Regex(pattern);
            return regex.Matches(inputString)
                .Cast<Match>()
                .Select(c => c.Value.Replace("\\", ""))//it's kind of a dirty hack. The quest: "put it inside the regexp".
                .ToList();
        }
        public static string ExtractCommandName(List<string> args)
        {
            if (args.Count == 0)
                throw new ArgumentException();
            var ans = args[0];
            args.RemoveAt(0);
            return ans.ToLower();
        }

    }
}
