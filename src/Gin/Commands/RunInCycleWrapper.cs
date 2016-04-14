using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class RunInCycleWrapper: CommandBase
    {
        readonly ICommand cmd;
        readonly int count;
        readonly Executor executor;
        public RunInCycleWrapper(ICommand cmd, int count, Executor executor)
        {
            this.cmd = cmd;
            this.count = count;
            this.executor = executor;
        }

        public override void Run() {
            for (int i = 0; i < count; i++) {
                Log.WriteMessage("\"" + ParseTools.NormalizeCommandTypeName(cmd.GetType().Name) + "\"'s iteration " + i + " of " + count);
                executor.Run(cmd);
            }
            Log.WriteMessage("Cycle finished");
        }
        public override ILog Log {
            get {
                return base.Log; 
            }
            set {
                base.Log = value;
                value.AttachTo(cmd);
            }
        }
    }
}
