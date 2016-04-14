using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class CommandSingletoneFactory : ICommandAbstractFactory
    {
        public readonly ICommand Exemplar;
        public CommandSingletoneFactory(ICommand readyToGoInstance)
        {
            this.Exemplar = readyToGoInstance;
        }
        public ICommand GetReadyToGoInstance()
        {
            return Exemplar;
        }
    }
}
