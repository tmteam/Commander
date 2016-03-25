using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander;
namespace Example
{
    [Command("Raise an exception")]
    public class RaiseAnException: CommandBase
    {
        public override void Run() {
            throw new Exception("Raised Exception");
        }
    }
}
