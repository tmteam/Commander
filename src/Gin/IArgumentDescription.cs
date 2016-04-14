using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin
{
    public interface IArgumentDescription
    {
        string Description { get; }
        bool Optional { get; }
        string ShortAlias { get; }
    }


}
