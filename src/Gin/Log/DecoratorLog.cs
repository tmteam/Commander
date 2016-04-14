using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class DecoratorLog : ILog
    {
        private readonly ILog[] logs;

        public DecoratorLog(params ILog[] logs)
        {
            this.logs = logs;
        }

        public void WriteError(string str)
        {
            foreach (ILog log in this.logs)
            {
                log.WriteError(str);
            }
        }

        public void WriteMessage(string str)
        {
            foreach (ILog log in this.logs)
            {
                log.WriteMessage(str);
            }
        }

        public void WriteWarning(string str)
        {
            foreach (ILog log in this.logs)
            {
                log.WriteWarning(str);
            }
        }
    }


}
