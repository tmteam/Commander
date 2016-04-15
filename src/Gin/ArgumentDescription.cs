using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// Description of a command argument
    /// </summary>
    public class ArgumentDescription
    {
        /// <summary>
        /// Atribute of a argument property
        /// </summary>
        public SettingAttribute Attribute;
        /// <summary>
        /// Argument property info
        /// </summary>
        public PropertyInfo Property;
    }
}
