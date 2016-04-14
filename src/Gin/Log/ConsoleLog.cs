using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin
{
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
