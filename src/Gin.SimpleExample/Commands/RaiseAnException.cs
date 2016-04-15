using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheGin;
namespace TheGin.SimpleExample
{
    [Command("Raises an exception")]
    public class RaiseAnException: CommandBase
    {
        public override void Run() {
            throw new Exception("Raised Exception");
        }
    }
}
