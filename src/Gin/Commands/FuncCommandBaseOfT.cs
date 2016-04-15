using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Base for commands which return some value
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class FuncCommandBase<TResult> : CommandBase, IFuncCommand<TResult>, IFuncCommand, ICommand
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
