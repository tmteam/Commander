using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    [Command("Close current application")]
    public class ExitCommand : CommandBase
    {
        private readonly Gin interpreter;

        public ExitCommand(Gin interpreter)
        {
            this.interpreter = interpreter;
        }

        public override void Run()
        {
            this.interpreter.ExitFlag = true;
        }
    }


}
