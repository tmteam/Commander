using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class UnknownArgumentException : Exception
    {
        // Fields
        public readonly string ArgumentName;

        // Methods
        public UnknownArgumentException(string argumentName)
        {
            this.ArgumentName = argumentName;
        }
    }

 

}
