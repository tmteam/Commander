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
        public void TestExtractCommandNameWhenNoArgs(){
            var args = new List<string> { "DiviDe" };
            var cmdName = ParseTools.ExtractCommandName(args);
            Assert.AreEqual("divide", cmdName);
            Assert.AreEqual(0, args.Count);
        }
        [TestMethod]
        public void TestExtractCommandNameWhenWithArgs()
        {
            var args = new List<string> { "DiviDe","a","4","b","2" };
            var cmdName = ParseTools.ExtractCommandName(args);
            Assert.AreEqual("divide", cmdName);
            Assert.AreEqual(4, args.Count);
        }
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
                 var splitted = ParseTools.SmartSplit(test.CommandString);
                 if (splitted.Count != test.Args.Length)
                     Assert.Fail("Parsed and expected lengths are not equal ("+ splitted.Count+" vs "+ test.Args.Length+")");
                 for (int i = 0; i < splitted.Count; i++) {
                     Assert.AreEqual(splitted[i], test.Args[i]);
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
        [TestMethod]
        public void TestTimeSpan()
        {
            var testCases = new Dictionary<string, TimeSpan>()
            {
                {"123", TimeSpan.FromMilliseconds(123)},
                {"2000", TimeSpan.FromSeconds(2)},
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

    class SomeObjectWithProperties
    {
        [CommandArgument(ShortAlias= "argi")]
        public int i { get; set; }
        [CommandArgument(ShortAlias= "someflag")]
        public bool flag { get; set; }
    }
    class SmartSplitCase
    {
        public string CommandString;
        public string[] Args;
 
    }
}
