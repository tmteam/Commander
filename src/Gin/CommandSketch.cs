using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Description of the single command 
    /// </summary>
    public class CommandSketch {
        public CommandSketch(CommandAttribute attribute, Type commandType, Func<ICommand> locator)
        {
            this.Arguments = ReflectionTools.GetArgumentsDescription(commandType);
            this.Attribute = attribute;
            this.CommandType = commandType;
            this._locator = locator;
        }
        /// <summary>
        /// Command's arguments description
        /// </summary>
        public readonly ArgumentDescription[] Arguments;
        /// <summary>
        /// The command attribute
        /// </summary>
        public readonly CommandAttribute Attribute;
        /// <summary>
        /// Type of the command
        /// </summary>
        public readonly Type CommandType;
        readonly Func<ICommand> _locator;
        /// <summary>
        /// Creates not configured instance of the command
        /// </summary>
        /// <returns></returns>
        public ICommand GetRawInstance() {
            return _locator();
        }
    }
}
