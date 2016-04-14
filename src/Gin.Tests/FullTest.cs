using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheGin;
using System.Collections.Generic;

namespace TheGin.Tests
{
    [TestClass]
    public class FullTest
    {
        [TestMethod]
        public void TestAll()
        {
          /*  var scanner = new TypeScanner();
            scanner.Registrate(typeof(TestAttributesCommand));
            var cmdString = "testattributes -int 42  -stringproperty \"forty two\" -flag";
            var factory = new CommandFactory(scanner);

            Tools.ParseToConsoleArgs(cmdString);
            var cmd = factory.CreateAndConfigure(cmdString, cmdString.Split(new[]{" "}, StringSplitOptions) as TestAttributesCommand;
            if (cmd == null)
                Assert.Fail("Command name was not parsed");
            Assert.AreEqual(42, cmd.IntProperty);
            Assert.AreEqual("forty two", cmd.StringProperty);
            Assert.AreEqual(true, cmd.BoolProperty);*/
        }
    }
}
