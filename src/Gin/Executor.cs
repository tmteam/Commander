using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Executes command in try-catch and writes its results to a specified log
    /// </summary>
    public class Executor : IExecutor
    {
        public Executor() {
            this.Log = new ConsoleLog();
        }
        public Executor(ILog log) {
            this.Log = log ?? new ConsoleLog();
        }
        public void Run(ICommand cmd){
            Log.TryAttachTo(cmd);
            try {
                cmd.Run();
                var func = cmd as IFuncCommand;
                if (func != null)
                    Console.WriteLine("Result: " + ReflectionTools.Describe(func.UntypedResult, "\t"));
            } catch (Exception ex) {
                Log.WriteError("Exception: \r\n" + ex.ToString());
            }
        }

        public ILog Log { get; set; }
    }

}
