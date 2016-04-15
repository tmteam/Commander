using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Unexpected argument is found
    /// </summary>
    public class UnknownArgumentsException : Exception
    {
        public readonly string[] ArgumentNames;

        public UnknownArgumentsException(params string[] argumentNames)
        {
            this.ArgumentNames = argumentNames;
        }
    }

 

}
