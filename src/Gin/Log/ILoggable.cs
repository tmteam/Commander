﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin {
    public interface ILoggable
    {
        // Properties
        ILog Log { get; set; }
    }
}
