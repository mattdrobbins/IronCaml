using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class Environment
    {
        public Environment enclosing;

        public Environment()
        {
        }

        private Dictionary<string, LinqExpressions.ParameterExpression> values = 
            new Dictionary<string, LinqExpressions.ParameterExpression>();

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public void Define(string name, ParameterExpression value)
        {
            values.Add(name, value);
        }

        public LinqExpressions.ParameterExpression Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            if (this.enclosing != null)
            {
                return this.enclosing.Get(name);
            }

            return null;
        }

    }
}
