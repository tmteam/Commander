using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class CommandRunProperties
    {
        [CommandArgument(ShortAlias= "at", Description= "", Optional = true)]
        public DateTime? At { get; set; }

        [CommandArgument(ShortAlias= "every", Description= "", Optional = true)]
        public TimeSpan? Every { get; set; }

        [CommandArgument(
            Optional = true, 
            ShortAlias = "count",
            Description = "max execution count")]
        public int? Count { get; set; }
        public bool IsEmpty {
            get { return !At.HasValue && !Every.HasValue; }
        }
    }
}
