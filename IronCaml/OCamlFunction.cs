using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class OCamlFunction : ICallable
    {
        private readonly Statement.Function _decleration;
        private readonly Dictionary<string, object> _closure;

        public OCamlFunction(Statement.Function decleration, Dictionary<string,object> closure)
        {
            _decleration = decleration;
            _closure = closure;         
        }

        public int Arity() => _decleration.Params.Count;

        public object Call(Interperater interperater, List<object> args)
        {
            var environment = new Dictionary<string, object>();

            for (int i = 0; i < _decleration.Params.Count; i++)
            {
                environment[_decleration.Params[i].Lexeme] = args[i];
            }

            return interperater.CallFunction(_decleration.Body, environment);
        }
    }
}
