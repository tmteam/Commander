using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin
{
    public class CommandDescription {
        public ArgumentDescription[] arguments;
        public CommandAttribute attribute;
        public ICommandAbstractFactory exemplarFactory;
        public Type type;
    }
}
