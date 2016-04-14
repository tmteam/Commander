using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string description)
        {
            this.Description = description;
        }
        public string Description { get; private set; }
    }


}
