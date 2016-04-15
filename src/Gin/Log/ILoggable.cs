using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin {

    /// <summary>
    /// Something that can writes to the log
    /// </summary>
    public interface ILoggable
    {
        ILog Log { get; set; }
    }
}
