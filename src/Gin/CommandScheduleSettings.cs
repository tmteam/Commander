using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Command execution settings
    /// </summary>
    public class CommandScheduleSettings
    {
        [Setting(ShortAlias= "at", Description= "specify first time of a command's execution", Optional = true)]
        public DateTime? At { get; set; }

        [Setting(ShortAlias= "every", Description= "specify interval between command's executions", Optional = true)]
        public TimeSpan? Every { get; set; }

        [Setting(
            Optional = true, 
            ShortAlias = "count",
            Description = "max count of executions")]
        public ulong? Count { get; set; }
        public bool IsEmpty {
            get { return !At.HasValue && !Every.HasValue; }
        }
    }
}
