using TheGin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin.ComplexExample
{
    class Program
    {
        static void Main(string[] args) {

            var scanner = new TypeScanner();
            //scan or append command types manualy here:
            //scanner.Registrate<myCommandType>();
            //or command singletone Instances
            //scanner.Registrate(new myCommand(myCommandSettings))
            
            //scan only executing assembly:            
            scanner.ScanAssembly(Assembly.GetEntryAssembly());

            var gin = new Gin(
                        library:  scanner,               //Specify your own command library
                        executor: new Executor(),        //(optional) specify your own command executor (do not forget to catch exceptions and attach the log to commands in it)
                        log:      new DecoratorLog       //(optional) combine different logs
                                  (
                                    new ConsoleLog(), 
                                    new FileLog(maxLogLength: 10000, 
                                                relativeFileName: "TheLog.txt",
                                                writeFilter: FileLogFilter.All)
                                                ));
           
            gin.Log.WriteMessage("Gin lauched at "+ DateTime.Now);
            
            //to switch log at runtime:
            //gin.Log = new ConsoleLog();
            if (!Environment.UserInteractive) //if we are launched as a service
            {
                gin.Log.WriteMessage("Executed as a service");
                //Do not to forget setup your service name at WindowsServiceInstaller.cs
 
                //and do whatever you want here as a service...

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
                //close the application after operation will be done
            } else { // when it's executed as a console application:
                gin.AddHelp();// add \"help\" command
                gin.AddExit();// add \"exit\" command

                gin.RunInputLoop();
                
                gin.Log.WriteMessage("Goodbye. Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
