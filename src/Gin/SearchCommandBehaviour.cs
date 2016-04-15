using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Default search behaviours
    /// </summary>
    public enum SearchCommandBehaviour {
        /// <summary>
        /// No commands will be searched
        /// </summary>
        DoNotSearch,
        /// <summary>
        /// The hole executing assembly will be searched for a suitable command types
        /// </summary>
        ScanExecutingAssembly,
        /// <summary>
        /// The hole executing assembly and all the referenced assemblies will be searched for a suitable command types
        /// </summary>
        ScanAllSolutionAssemblies,
    }
}
