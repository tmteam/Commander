using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class RunAsThreadWrapper : CommandBase
    {
        private CommandRunProperties properties;
        private ICommand command;
        private Scheduler scheduler;

        public RunAsThreadWrapper(Scheduler scheduler, CommandRunProperties properties, ICommand command)
        {
            this.scheduler = scheduler;
            this.command = command;
            this.properties = properties;
        }

        public override void Run() {
            this.scheduler.AddTask(new SingletoneCommandAbstractFactory(command), properties);
        }
        public override ILog Log
        {
            get
            {
                return base.Log;
            }
            set
            {
                base.Log = value;
                var loggable = command as ILoggable;
                if (loggable != null)
                    loggable.Log = value;
            }
        }
    }


}
