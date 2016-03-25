using Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static int Main(string[] args)
        {

            var interpreter = new Interpreter(
                        scanner: new TypeScanner(), 
                        log:     new DecoratorLog( 
                                    new ConsoleLog(), 
                                    new FileLog(10000, "TheLog.txt")));
            
            interpreter.Log.WriteMessage("Commander Example");
            
            interpreter.CommandInformation.ScanAssembly(Assembly.GetEntryAssembly());
            
            if (args.Length > 0)
                interpreter.Execute(args);
            else {
                interpreter.AddExitCommand();
                interpreter.AddHelpCommand();
                interpreter.Execute("help");
                /*
                interpreter.Execute(new DivideCommand
                {
                    Divider = 100,
                    Dividend = 2
                }, TimeSpan.FromSeconds(10), null, 10);
                */
                interpreter.RunInputLoop();
            }
            
            interpreter.Log.WriteMessage("Goodbye");
            Console.ReadLine();
            return 0;
        }
    }
}
