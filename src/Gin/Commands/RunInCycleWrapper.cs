using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class RunInCycleWrapper: CommandBase
    {
        readonly CommandFactory factory;
        readonly int count;
        readonly Executor executor;
        public RunInCycleWrapper(CommandFactory factory, int count, Executor executor)
        {
            this.factory = factory;
            this.count = count;
            this.executor = executor;
        }

        public override void Run() {
            for (int i = 0; i < count; i++) {
                var cmd = factory.GetReadyToGoInstance();
                Log.WriteMessage("\"" + ParseTools.GetCommandName(cmd.GetType()) + "\"'s iteration " + i + " of " + count);
                executor.Run(cmd);
            }
            Log.WriteMessage("Cycle finished");
        }
    }
}
