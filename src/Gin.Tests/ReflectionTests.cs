using Gin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin.Tests
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void IsNullableTest()
        {
            var testCases = new Dictionary<Type, bool>{
                {typeof(int), false},
                {typeof(int?), true},
                {typeof(string), false},
                {typeof(ReflectionTests), false},
                {typeof(decimal?), true},
            };
            foreach (var test in testCases)
            {
                Assert.AreEqual(test.Value, ReflectionTools.IsNullable(test.Key));
            }
        }
        [TestMethod]
        public void GetNonNullableTest()
        {
            var testCases = new Dictionary<Type, Type>{
                {typeof(int), typeof(int)},
                {typeof(int?), typeof(int)},
                {typeof(string), typeof(string)},
                {typeof(ReflectionTests), typeof(ReflectionTests)},
                {typeof(decimal?), typeof(decimal)},
            };
            foreach (var test in testCases)
            {
                Assert.AreEqual(test.Value, ReflectionTools.GetNonNullType(test.Key));
            }
        }
    }
}
