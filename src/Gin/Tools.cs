using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public static class Tools
    {
        /// <summary>
        /// Tries to attach log to an object 
        /// </summary>
        public static bool TryAttachTo(this ILog log,object obj)
        {
            var loggable = obj as ILoggable;
            if (loggable != null)
                loggable.Log = log;
            return loggable != null;
        }
        public static ILog GetLogOrNull(this object obj)
        {
            if (obj is ILoggable)
                return (obj as ILoggable).Log;
            return null;
        }
        /// <summary>
        /// Adds the help command to the Gin's library
        /// </summary>
        /// <param name="gin"></param>
        public static void AddHelp(this Gin gin) {
            gin.Library.Registrate(new HelpCommand(gin.Library.Sketches));
        }
        /// <summary>
        /// Adds the exit command to the Gin's library
        /// </summary>
        /// <param name="gin"></param>
        public static void AddExit(this Gin gin) {
            gin.Library.Registrate(new ExitCommand(gin));
        }
        /// <summary>
        /// Runs command input loop.
        /// </summary>
        /// <param name="gin"></param>
        public static void RunInputLoop(this Gin gin)
        {
            while (!gin.NeedToExit) {
                Console.Write("\r\n> ");
                var command = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(command))
                    gin.Execute(command);
            }
        }
    }
}
