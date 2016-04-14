using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class Executor
    {
        readonly ILog log;
        public Executor(ILog log)
        {
            this.log = log;
        }
        public void Run(ICommand cmd){
            log.AttachTo(cmd);
            try {
                cmd.Run();
                var func = cmd as IFuncCommand;
                if (func != null)
                    Console.WriteLine("Result: " + ReflectionTools.Describe(func.UntypedResult, "\t"));
            } catch (Exception ex) {
                log.WriteError("Exception: \r\n" + ex.ToString());
            }
        }
    }

}
