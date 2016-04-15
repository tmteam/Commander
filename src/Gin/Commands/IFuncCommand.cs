using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Something that can be executed and returns something as a result
    /// </summary>
    public interface IFuncCommand : ICommand
    {
        object UntypedResult { get; }
    }
}
