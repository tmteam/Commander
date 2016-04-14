using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin
{
    public abstract class CommandBase : ICommand, ILoggable
    {
        // Methods
        public CommandBase()
        {
            this.Log = new ConsoleLog();
        }

        public abstract void Run();

        // Properties
        public virtual ILog Log { get; set; }
    }

    public abstract class CommandBase<TResult> : CommandBase, ICommand<TResult>, IFuncCommand, ICommand
    {

    public override void Run()
    {
        this.TypedResult = this.RunAndReturn();
    }

    protected abstract TResult RunAndReturn();

    // Properties
    public TResult TypedResult { get; protected set; }

    public object UntypedResult { get { return this.TypedResult; } }
}


}
