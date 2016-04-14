using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class CommandDescription {
        public ArgumentDescription[] Arguments;
        public CommandAttribute Attribute;
        public ICommand Exemplar;
    }
}
