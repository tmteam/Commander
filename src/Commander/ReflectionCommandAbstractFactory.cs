using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
  public class ReflectionCommandAbstractFactory : ICommandAbstractFactory
{
    public readonly Type Type;
    public ReflectionCommandAbstractFactory(Type type)
    {
        this.Type = type;
    }

    public ICommand GetExemplar() {
        return ((ICommand)Activator.CreateInstance(this.Type));
    }
}

 

}
