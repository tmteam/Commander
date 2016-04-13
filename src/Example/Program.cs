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
        static void Main(string[] args) {

            var interpreter = new Interpreter(
                        scanner: new TypeScanner(), 
                        log:     new DecoratorLog( 
                                    new ConsoleLog(), 
                                    new FileLog(10000, "TheLog.txt")));
            
            interpreter.Log.WriteMessage("Commander lauched");
            
            interpreter.CommandInformation.ScanAssembly(Assembly.GetEntryAssembly());

            if (!Environment.UserInteractive)
            {
                interpreter.Log.WriteMessage("Executed as a service");
            
                interpreter.Execute("divide  a 10  b 5  at 02:00  every 24h");
                interpreter.Execute("writeHello");
                
                interpreter.WaitForFinshed();
            } else if (args.Length > 0) { // when it's executed with parameters:
                interpreter.Execute(args);
                return;
            } else { // when it's executed as console application:

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
                interpreter.Log.WriteMessage("Goodbye. Press any key to continue...");
                Console.ReadLine();
                return;
            }
        }
    }
}
