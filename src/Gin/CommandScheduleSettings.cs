using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class CommandScheduleSettings
    {
        [Setting(ShortAlias= "at", Description= "", Optional = true)]
        public DateTime? At { get; set; }

        [Setting(ShortAlias= "every", Description= "", Optional = true)]
        public TimeSpan? Every { get; set; }

        [Setting(
            Optional = true, 
            ShortAlias = "count",
            Description = "max execution count")]
        public int? Count { get; set; }
        public bool IsEmpty {
            get { return !At.HasValue && !Every.HasValue; }
        }
    }
}
