using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class TaskPlan
    {
        public ICommandAbstractFactory CommandFactory;
        public int intervalInMsec;
        public DateTime plannedTime;
        public int executedCount = 0;
        public int? maxExecutionCount = null;
    }


}
