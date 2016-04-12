using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Commander;

namespace Commander.Tests
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void TestSmartSplit()
        {
             var cases = new[]{
                
                new SmartSplitCase{
                    CommandString = " qwert ",
                    Args = new[] {"qwert"}
                },
                new SmartSplitCase{
                    CommandString = "qwe qwe",
                    Args = new[] {"qwe", "qwe"}
                },
                new SmartSplitCase{
                    CommandString = "primitive",
                    Args = new []{"primitive"}    
                },
                new SmartSplitCase{

                     CommandString = "romdomdomCmd  dom2 Dom \"Aleksandr \\\" \\\" pushkin\\\"\" \"lalala\" rom 14 ",
                     Args = new[]{
                         "romdomdomCmd",  "dom2", "Dom", "\"Aleksandr \" \" pushkin\"\"" ,"\"lalala\"","rom","14"
                     }
                },
            };
             foreach (var test in cases) {
                 var splitted = Tools.SmartSplit(test.CommandString);
                 if (splitted.Count != test.Args.Length)
                     Assert.Fail("Parsed and expected lengths are not equal ("+ splitted.Count+" vs "+ test.Args.Length+")");
                 for (int i = 0; i < splitted.Count; i++) {
                     Assert.AreEqual(splitted[i], test.Args[i]);
                 }
             }
        }
        [TestMethod]
        public void TestInputArguments()
        {
            var testCases = new CommandParseTestCase[]{
                new CommandParseTestCase{
                    CommandString = "romdomdomCmd  dom2 Dom \"Александр\\\"\\\" Пушкин\\\"\" rom 14 ",
                    IsValid = true, 
                    CommandName = "romdomdomcmd",
                      Args = new Dictionary<string,string>{
                           {"rom", "14"},
                           {"dom", "Александр\"\" Пушкин\""},
                           {"dom2", true.ToString()},
                      },
                },
                new CommandParseTestCase{
                    CommandString = " romdomdomCmd rom dom dom2 ",
                    IsValid = true, 
                    CommandName = "romdomdomcmd",
                    Args = new Dictionary<string,string>{
                           {"rom",  true.ToString()},
                           {"dom",  true.ToString()},
                           {"dom2", true.ToString()},
                      },
                },
                new CommandParseTestCase{
                    CommandString = " romdomdomCmd bubuh rom dom dom2 ",
                    IsValid = false, 
                    CommandName = "romdomdomcmd",
                },
                new CommandParseTestCase{
                    CommandString = " ",
                    IsValid = false, 
                },
                new CommandParseTestCase{
                    CommandString = "  BurumBurum",
                    IsValid = true, 
                    CommandName = "burumburum",
                    Args = new Dictionary<string,string>(),
                },
            };

            foreach (var someCase in testCases) {
                string cmdName;
                Dictionary<string, string> args;
                var consoleArgs = Tools.ParseToConsoleArgs(someCase.CommandString);
                bool isValid = Tools.TryParse(consoleArgs, out cmdName, out args);
                Assert.AreEqual(someCase.IsValid, isValid);
                if (!isValid)
                    continue;
                Assert.AreEqual(cmdName, someCase.CommandName);
                foreach (var expected in someCase.Args) {
                    if (!args.ContainsKey(expected.Key))
                        Assert.Fail("argument " + expected.Key + " not found");

                    if(args[expected.Key]!= expected.Value)
                        Assert.Fail("value of " + expected.Key + " is not parsed correctly");
                }

                foreach (var arg in args) {
                    if (!someCase.Args.ContainsKey(arg.Key))
                        Assert.Fail("unexpected " + arg.Key);
                }
            }   
        }
        [TestMethod]
        public void TestDateTime()
        {
            var testCases = new Dictionary<string, DateTime>(){
                {new DateTime(2015,01,02,03,04,05).ToString(), new DateTime(2015,01,02,03,04,05)},
                {"01.12",       new DateTime(DateTime.Now.Year, 12,01)},
                {"01.12.2017",  new DateTime(2017, 12,01)},
                {"01:23",       new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,01,23,00)},
                {"01:23:45",    new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,01,23,45)},
                {"14:23:22",    new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,14,23,22)},
                {"01.12 01:02", new DateTime(DateTime.Now.Year, 12,01,01,02,00)},
                {"01.12.2005 01:02:03", new DateTime(2005, 12,01,01,02,03)},
            };

            foreach (var testCase in testCases)
                Assert.AreEqual(testCase.Value, new DateTimeValue(testCase.Key).ToDateTime());
        }
        public void TestTimeSpan()
        {
            var testCases = new Dictionary<string, TimeSpan>()
            {
                {"123", TimeSpan.FromMilliseconds(123)},
                {"123s", TimeSpan.FromSeconds(123)},
                {"123m", TimeSpan.FromMinutes(123)},
                {"123h", TimeSpan.FromHours(123)},
                {"123d", TimeSpan.FromDays(123)},
                {"123w", TimeSpan.FromDays(123*7)},
            };
            foreach (var testCase in testCases)
                Assert.AreEqual(testCase.Value, new DateTimeValue(testCase.Key).ToTimeSpan());
        }
    }

    class CommandParseTestCase{
        public bool IsValid;
        public string CommandString;
        public string CommandName;
        public Dictionary<string, string> Args;
    };
    class SmartSplitCase
    {
        public string CommandString;
        public string[] Args;
 
    }
}
