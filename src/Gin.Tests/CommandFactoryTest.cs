using TheGin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TheGin.Tests
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
            if (CreateFactory().Sketches
                .FirstOrDefault(d => d.Attribute.Description == "someCommandDescription") == null)
                Assert.Fail("Got no command description");
        }
        [TestMethod] public void TestCommandArgumentsDescription()
        {
            var description = CreateFactory().Sketches
                .FirstOrDefault(d => d.Attribute.Description == "someCommandDescription");
            
            var intDescription = description.Arguments
                .FirstOrDefault(a => a.Attribute is SettingAttribute
                    && a.Attribute.Description == "someIntegerDescription" 
                    && a.Attribute.ShortAlias == "int"
                    && a.Property.PropertyType == typeof(int)
                    && a.Attribute.Optional);
            
            if (intDescription == null)
                Assert.Fail("Got no correct int description");


            var strDescription = description.Arguments
                .FirstOrDefault(a => a.Attribute is SettingAttribute
                    && a.Attribute.Description == "someStringDescription"
                    && a.Attribute.ShortAlias == "str"
                    && a.Property.PropertyType == typeof(string)
                    && !a.Attribute.Optional);
            if (strDescription == null)
                Assert.Fail("Got no correct str description");
        }
    }
}
