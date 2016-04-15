using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class CommandSketch {
        public CommandSketch(CommandAttribute attribute, Type commandType, Func<ICommand> locator)
        {
            this.Arguments = ReflectionTools.GetArgumentsDescription(commandType);
            this.Attribute = attribute;
            this.CommandType = commandType;
            this._locator = locator;
        }
        public readonly ArgumentDescription[] Arguments;
        public readonly CommandAttribute Attribute;
        public readonly Type CommandType;
        readonly Func<ICommand> _locator;
        public ICommand GetRawInstance() {
            return _locator();
        }
    }
}
