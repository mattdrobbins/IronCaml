using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public interface ICallable
    {
        object Call(Interperater interperater, List<object> args);
        int Arity();
    }
}
