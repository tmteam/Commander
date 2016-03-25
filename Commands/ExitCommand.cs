using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    [Command("Close current application")]
    public class ExitCommand : CommandBase
    {
        private readonly Interpreter interpreter;

        public ExitCommand(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public override void Run()
        {
            this.interpreter.ExitFlag = true;
        }
    }


}
