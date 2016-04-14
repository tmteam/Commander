using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin
{
    public class RunInCycleWrapper: CommandBase
    {
        readonly ICommand command;
        readonly int count;
        readonly Action<ICommand> executor;
        public RunInCycleWrapper(ICommand command, int count, Action<ICommand> executor)
        {
            this.command = command;
            this.count = count;
            this.executor = executor;
        }

        public override void Run() {
            for (int i = 0; i < count; i++) {
                Log.WriteMessage("\"" + ParseTools.NormalizeCommandTypeName(command.GetType().Name) + "\"'s iteration " + i + " of " + count);
                executor(command);
            }
            Log.WriteMessage("Cycle finished");
        }
        public override ILog Log {
            get {
                return base.Log; 
            }
            set {
                base.Log = value;
                var loggable = command as ILoggable;
                if (loggable != null)
                    loggable.Log = value;
            }
        }
    }
}
