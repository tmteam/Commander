using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class CommandSketch {
        public CommandSketch(CommandAttribute attribute, Type commandType, Func<ICommand> factory)
        {
            this.Arguments = ReflectionTools.GetArgumentsDescription(commandType);
            this.Attribute = attribute;
            this.CommandType = commandType;
            this._factory = factory;
        }
        public readonly ArgumentDescription[] Arguments;
        public readonly CommandAttribute Attribute;
        public readonly Type CommandType;
        readonly Func<ICommand> _factory;
        public ICommand GetRawInstance() {
            return _factory();
        }
    }
}
