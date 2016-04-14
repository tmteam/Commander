using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Gin;

namespace Gin.Tests
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullableFailConvert(){
            ParseTools.Convert("123", typeof(int?), "testArg");
        }
        
        [TestMethod]
        public void TestFailConvert()
        {
            var testCases = new Dictionary<string, Type>()
            {
                 {"some string", typeof(bool)},
                 {"123a",       typeof(int)},
                 {"\"100",      typeof(TimeSpan)},
                 {"23:35.123",  typeof(TimeSpan)},
                 {"hurray!",    typeof(DateTime)},
                 {"",    typeof(int)},

            };
            foreach(var test in testCases){
                try{
                    ParseTools.Convert(test.Key, test.Value, "testArg");
                }
                catch(InvalidArgumentException ex){
                    continue; //This's ok!
                }
                catch (Exception ex)
                {
                    //And this is not...
                    Assert.Fail("Exception with incorrect type "+ ex.GetType().Name+" raised for " + test.Key + " to " + test.Value.Name + " convertion");
                }
                //And this...
                Assert.Fail("InvalidArgumentException did not raise for " + test.Key + " to " + test.Value.Name + " convertion");
            }
        }
        [TestMethod] 
        public void TestSuccesfullConvert(){
            var testCases = new Dictionary<string, object>()
            {
                 {"some string", "some string"},
                 {"\"quote\"", "quote"},
                 {"123",    123},
                 {"100",    TimeSpan.FromMilliseconds(100)},
                 {"23:35",  TimeSpan.FromMinutes(23*60 + 35)},
                 {"23:40",  DateTime.Now.Date+ TimeSpan.FromMinutes(23*60+ 40)},
                 {"No",        false},
                 {"yes",       true},
                 {"TRUE",      true},
                 {"false",     false},
                 {"\"1\"",     true},
                 {"0",         false},
                 {"101",       (double)101},
                 {"\"103\"",   (float)103},
                 {"123,7",     (double)123.7},
                 {"123.5",     (double)123.5},
                 {"123.555",   123.555M},
                 {"124,555",   124.555M},
                 {"\"124.5\"", (float)124.5},
                 {"125,5",     (float)125.5},
                 
            };
            foreach(var test in testCases){
                Assert.AreEqual(test.Value, ParseTools.Convert(test.Key, test.Value.GetType(),"arg"));
            }
        }
        [TestMethod]
        public void TestNormalizeCommandTypeName()
        {
            Assert.AreEqual("cmd", ParseTools.NormalizeCommandTypeName("cmd"));
            Assert.AreEqual("SomeThing", ParseTools.NormalizeCommandTypeName("SomeThing"));
            Assert.AreEqual("Divide", ParseTools.NormalizeCommandTypeName("DivideCommand"));
        }
        [TestMethod]
        public void TestNormalizeCommandArgName()
        {
            Assert.AreEqual("arg1", ParseTools.NormalizeCommandArgName("ArG1"));
            Assert.AreEqual("arg2", ParseTools.NormalizeCommandArgName("ArG2:"));
            Assert.AreEqual("arg3", ParseTools.NormalizeCommandArgName("-ArG3"));
        }
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
