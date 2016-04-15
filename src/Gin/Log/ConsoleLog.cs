using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// The log which writes to the console
    /// </summary>
    public class ConsoleLog : ILog
    {
        public void WriteError(string str)
        {
            Console.WriteLine("[ERROR] " + str);
        }

        public void WriteMessage(string str)
        {
            Console.WriteLine(str);
        }

        public void WriteWarning(string str)
        {
            Console.WriteLine("[warning] " + str);
        }
    }
}
