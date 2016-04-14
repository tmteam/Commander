using TheGin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin.Example
{



    class Program
    {
        static void Main(string[] args) {

            var scanner = new TypeScanner();
            scanner.ScanAssembly(Assembly.GetEntryAssembly());
            var gin = new Gin(
                        library: scanner, 
                        log:     new DecoratorLog( 
                                    new ConsoleLog(), 
                                    new FileLog(10000, "TheLog.txt")));

            gin.AddHelp();
            gin.AddExit();

            gin.Log.WriteMessage("Gin lauched");

            if (!Environment.UserInteractive)
            {
                gin.Log.WriteMessage("Executed as a service");
                //Do whatever you want here as a service...

                //Will be executed in scheduler's timer thread:
                gin.Execute("divide  a 10  b 5  at 02:00  every 24h");
                //You can use different argument styles and combine them
                //gin.Execute("divide a: 10  b: 5  at 02:00  every 24h");
                //gin.Execute("divide -a 10  -b 5  -at 02:00  every \"24h\"");
                
                //Will be executed at this thread:
                gin.Execute("writeHello");
                
                gin.WaitForFinsh();
            } else if (args.Length > 0) { // when it's executed with parameters:
                gin.Execute(args);
                
                gin.Log.WriteMessage("Goodbye. Press any key to continue...");
                Console.ReadLine();
            } else { // when it's executed as a console application:

                //interpreter.AddExitCommand();
                //interpreter.AddHelpCommand();
                gin.Execute("help");
                
                gin.RunInputLoop();
                
                gin.Log.WriteMessage("Goodbye. Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
