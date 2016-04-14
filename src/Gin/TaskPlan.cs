using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin {
    public class TaskPlan
    {
        public Instruction Instruction;
        public int ExecutedCount = 0;
        public DateTime? PlannedTime;
    }
}
