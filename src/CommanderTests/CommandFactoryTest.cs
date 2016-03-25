using Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Commander.Tests
{
    [TestClass]
    public class CommandFactoryTest
    {
        TypeScanner CreateFactory()
        {
            var factory = new TypeScanner();
            factory.Registrate(typeof(TestAttributesCommand));
            return factory;
        }
        [TestMethod] public void TestCommandAttribute()
        {
            if (CreateFactory().Descriptions
                .FirstOrDefault(d => d.attribute.Description == "someCommandDescription") == null)
                Assert.Fail("Got no command description");
        }
        [TestMethod] public void TestCommandArgumentsDescription()
        {
            var description = CreateFactory().Descriptions
                .FirstOrDefault(d => d.attribute.Description == "someCommandDescription");
            
            var intDescription = description.arguments
                .FirstOrDefault(a => a.Description is CommandArgumentAttribute
                    && a.Description.Description == "someIntegerDescription" 
                    && a.Description.ShortAlias == "int"
                    && a.Property.PropertyType == typeof(int)
                    && a.Description.Optional);
            
            if (intDescription == null)
                Assert.Fail("Got no correct int description");


            var strDescription = description.arguments
                .FirstOrDefault(a => a.Description is CommandArgumentAttribute
                    && a.Description.Description == "someStringDescription"
                    && a.Description.ShortAlias == "str"
                    && a.Property.PropertyType == typeof(string)
                    && !a.Description.Optional);
            if (strDescription == null)
                Assert.Fail("Got no correct str description");

            var flagDescription = description.arguments
                .FirstOrDefault(a => a.Description is FlagArgumentAttribute
                    && a.Description.Description == "someFlagDescription"
                    && a.Description.ShortAlias == "flag"
                    && a.Property.PropertyType == typeof(bool) 
                    && !a.Description.Optional);
            if (flagDescription == null)
                Assert.Fail("Got no correct flag description");
        }
    }
}
