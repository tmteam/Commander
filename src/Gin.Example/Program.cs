using Gin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gin.Example
{



    class Program
    {
        /*
        CommandFactory - создаёт экземпляр команды.
        CommandScheduleSettings - настройки комманды

        TaskSettings { 
	        CommandFactory
	        CommandScheduleSettings
        }


        interpritator   - ковертирует строки в описание команд и параметров их запуска.
        handler         - запускает одну команду, согласно её описанию
        scheduler       - вызывает хендлер согласно заданным параметрам расписания
        Executor(Worker, TheManager) - внешний фасад команд.
        */

        static void Main(string[] args) {

            var interpreter = new Interpreter(
                        scanner: new TypeScanner(), 
                        log:     new DecoratorLog( 
                                    new ConsoleLog(), 
                                    new FileLog(10000, "TheLog.txt")));
            
            interpreter.Log.WriteMessage("Gin lauched");
            
            interpreter.CommandInformation.ScanAssembly(Assembly.GetEntryAssembly());

            if (!Environment.UserInteractive)
            {
                interpreter.Log.WriteMessage("Executed as a service");
                //Do whatever you want here as a service...

                //Will be executed in scheduler's timer thread:
                interpreter.Execute("divide  a 10  b 5  at 02:00  every 24h");
                //You can use different argument styles and combine them
                //interpreter.Execute("divide a: 10  b: 5  at 02:00  every 24h");
                //interpreter.Execute("divide -a 10  -b 5  -at 02:00  every \"24h\"");
                
                //Or you can launch it manualy:
                /*
                interpreter.Execute( 
                    cmd: new DivideCommand
                    {
                        Divider = 100,
                        Dividend = 2
                    }, 
                    interval: TimeSpan.FromSeconds(10),
                    count:      10);
                */

                //Will be executed in this thread:
                interpreter.Execute("writeHello");
                
                interpreter.WaitForFinsh();
            } else if (args.Length > 0) { // when it's executed with parameters:
                interpreter.Execute(args);
                
                interpreter.Log.WriteMessage("Goodbye. Press any key to continue...");
                Console.ReadLine();
            } else { // when it's executed as a console application:

                interpreter.AddExitCommand();
                interpreter.AddHelpCommand();
                interpreter.Execute("help");
                
                interpreter.RunInputLoop();
                
                interpreter.Log.WriteMessage("Goodbye. Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
