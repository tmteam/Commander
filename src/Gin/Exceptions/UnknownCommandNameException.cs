using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class UnknownCommandNameException : Exception
    {
        public readonly string CommandName;
        public UnknownCommandNameException(string commandName)
        {
            this.CommandName = commandName;
        }
    }


}
