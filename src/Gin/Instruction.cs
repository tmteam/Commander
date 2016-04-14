using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class Instruction
    {
        public ICommand ConfiguredCommand { get; set; }
        public CommandRunProperties ScheduleProperties { get; set; }
    }
}
