using TheGin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin.SimpleExample
{
    [Command("Writes word \"hello\" and nothing more")]
    public class WriteHelloCommand: CommandBase
    {
        public override void Run() {
            Log.WriteMessage("Hello!");
        }
    }
}
