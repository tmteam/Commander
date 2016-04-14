using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public interface ILog
    {
        void WriteError(string str);
        void WriteMessage(string str);
        void WriteWarning(string str);
    }
}
