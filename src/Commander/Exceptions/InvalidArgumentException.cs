using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class InvalidArgumentException:Exception
    {
        public readonly Type ExpectedType;
        public readonly string ActualValue;
        public readonly string ArgumentAlias;
        public InvalidArgumentException(Type expectedType, string actualValue, string argumentAlias)
            : base("Cannot convert value \"" + actualValue + "\" to type \""
            +((expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(Nullable<>)?expectedType.GenericTypeArguments.First():expectedType).Name
            +"\" for argument \"-"+ argumentAlias+"\"" ))
        {
            this.ExpectedType = expectedType;
            this.ActualValue = actualValue;
            this.ArgumentAlias = argumentAlias;
        }
    }
}
