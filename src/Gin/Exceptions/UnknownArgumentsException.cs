using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin
{
    public class UnknownArgumentsException : Exception
    {
        // Fields
        public readonly string[] ArgumentNames;

        // Methods
        public UnknownArgumentsException(params string[] argumentNames)
        {
            this.ArgumentNames = argumentNames;
        }
    }

 

}
