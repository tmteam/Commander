using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public static class Tools
    {
        public static void AttachTo(this ILog log,ICommand cmd)
        {
            var loggable = cmd as ILoggable;
            if (loggable != null)
                loggable.Log = log;
        }
        public static void AddHelp(this Gin gin) {
            gin.Library.Registrate(new HelpCommand(gin.Library.Sketches));
        }
        public static void AddExit(this Gin gin) {
            gin.Library.Registrate(new ExitCommand(gin));
        }
        public static void RunInputLoop(this Gin gin)
        {
            while (!gin.ExitFlag) {
                Console.Write("\r\n> ");
                var command = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(command))
                    gin.Execute(command);
            }
        }
    }
}
