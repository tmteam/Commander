using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class MissedArgumentsException : Exception
    {
        public readonly string[] ArgumentsName;
        public MissedArgumentsException(string[] argumentsName)
        {
            this.ArgumentsName = argumentsName;
        }
    }
}
