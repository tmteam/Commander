using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Command launch intruction
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// Provides a method to access the configured ready-to-go instance(s) of the command 
        /// </summary>
        public CommandLocator Locator { get; set; }
        /// <summary>
        /// Additional launch settings
        /// </summary>
        public CommandScheduleSettings ScheduleSettings { get; set; }
    }
}
