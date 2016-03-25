using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class SingletoneCommandAbstractFactory : ICommandAbstractFactory
    {
        public readonly ICommand Singletone;

        public SingletoneCommandAbstractFactory(ICommand exemplar) {
            this.Singletone = exemplar;
        }

        public ICommand GetExemplar() {
           return this.Singletone;
        }
    }


}
