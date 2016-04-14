using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class CommandFactory : ICommandAbstractFactory
    {
        readonly Func<ICommand> _factory;
        readonly Dictionary<PropertyInfo, object> _configuration;
        public CommandFactory(Func<ICommand> factory, Dictionary<PropertyInfo, object> configuration)
        {
            this._configuration = configuration;
            this._factory = factory;
        }
        public ICommand GetReadyToGoInstance()
        {
            var ans = _factory();
            ReflectionTools.Configurate(ans, _configuration);
            return ans;
        }
    }
}
