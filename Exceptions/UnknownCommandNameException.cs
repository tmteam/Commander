using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class UnknownCommandNameException : Exception
    {
        // Fields
        public readonly string CommandName;

        // Methods
        public UnknownCommandNameException(string commandName)
        {
            this.CommandName = commandName;
        }
    }


}
