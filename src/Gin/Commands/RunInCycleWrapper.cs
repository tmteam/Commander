using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Laucnhes specified command in a cycle
    /// </summary>
    public class RunInCycleWrapper: CommandBase, ICommand
    {
        readonly CommandLocator locator;
        readonly ulong iterationsCount;
        readonly IExecutor executor;
        public RunInCycleWrapper(CommandLocator locator, ulong iterationsCount, IExecutor executor)
        {
            this.locator = locator;
            this.iterationsCount = iterationsCount;
            this.executor = executor;
        }

        public override void Run() {
            for (ulong i = 0; i < iterationsCount; i++) {
                var cmd = locator.GetReadyToGoInstance();
                Log.WriteMessage("\"" + ParseTools.GetCommandName(cmd.GetType()) + "\"'s iteration " + i + " of " + iterationsCount);
                executor.Run(cmd);
            }
            Log.WriteMessage("Cycle finished");
        }
        
    }
}
