﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class Instruction
    {
        public CommandLocator Locator { get; set; }
        public CommandScheduleSettings ScheduleSettings { get; set; }
    }
}
