using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public interface ICommand
    {
        void Run();
    }

    public interface ICommand<TResult> : IFuncCommand, ICommand
    {
        TResult TypedResult { get; }
    }
}
