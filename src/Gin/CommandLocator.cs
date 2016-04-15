using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class CommandLocator 
    {
        readonly Func<ICommand> _instanceLocator;
        readonly Dictionary<PropertyInfo, object> _configuration;
        public CommandLocator(Func<ICommand> instanceLocator, Dictionary<PropertyInfo, object> configuration =null) {
            this._configuration     = configuration?? new Dictionary<PropertyInfo, object>();
            this._instanceLocator   = instanceLocator;
        }
        /// <summary>
        /// Returns configurated and ready for execution exemplar of a command
        /// </summary>
        public ICommand GetReadyToGoInstance()
        {
            var ans = _instanceLocator();
            ReflectionTools.Configurate(ans, _configuration);
            return ans;
        }
    }
}
