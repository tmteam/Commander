﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Base for commands which return nothing (procedures)
    /// </summary>
    public abstract class CommandBase : ICommand, ILoggable
    {
        // Methods
        public CommandBase()
        {
            this.Log = new ConsoleLog();
        }

        public abstract void Run();

        // Properties
        public virtual ILog Log { get; set; }
    }

    


}
