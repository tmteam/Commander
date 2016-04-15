using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Necessary argument is missing
    /// </summary>
    public class MissedArgumentsException : Exception
    {
        public readonly string[] ArgumentNames;
        public MissedArgumentsException(string[] argumentNames)
        {
            this.ArgumentNames = argumentNames;
        }
    }
}
